<template>
    <div class="pt-2">
        <div class="form-group">
            <label for="inputName">Name</label>
            <input name="agentName" formControlName="agentName" class="form-control" id="agentName" v-model="agent.name">
        </div>
        <div class="form-group">
            <label for="inputName">Repository</label>
            <input name="agentRepository" formControlName="agentRepository" class="form-control" id="agentRepository" v-model="agent.repository">
        </div>
        <div class="form-group">
            <label for="inputCategory">Category</label>
            <vue-tags-input v-model="tag" placeholder="Add Category" :tags="tags" :autocomplete-items="filteredItems" @tags-changed="newTags => tags = newTags" />
        </div>
        <div class="form-group">
            <label for="inputCmd">Command <a href="https://docs.reconness.com/agents/add-agent#add-new-agent" target="_blank">Learn more</a></label>
            <input name="command" formControlName="command" class="form-control" id="command" v-model="agent.command">
        </div>
        <hr />
        <h4>Agent Type</h4>
        <div class="form-group form-check">
            <input class="form-check-input" type="checkbox" id="isByTarget" v-model="agent.isByTarget">
            <label class="form-check-label" for="isByTarget">
                Target
            </label>
        </div>
        <div class="form-group form-check">
            <input class="form-check-input" type="checkbox" id="isByRootDomain" v-model="agent.isByRootDomain">
            <label class="form-check-label" for="isByRootDomain">
                RootDomain
            </label>
        </div>
        <div class="form-group form-check">
            <input class="form-check-input" type="checkbox" id="isBySubdomain" v-model="agent.isBySubdomain">
            <label class="form-check-label" for="isBySubdomain">
                Subdomains
            </label>
        </div>
        <hr />
        <h4>Script</h4>
        <div class="form-group">
            <label for="inputArguments"><a href="https://docs.reconness.com/agents/script-agent">Learn more</a></label>
            <editor v-model="agent.script" @init="editorInit" lang="csharp" theme="dracula" width="800" height="600"></editor>
        </div>
        <div class="form-group">
            <button class="btn btn-primary" v-if="isNew" v-on:click="onSave" :disabled='!isValid()'>Add</button>
            <button class="mt-2 btn btn-primary" v-if="!isNew" v-on:click="onUpdate()">Update</button>
            <button class="mt-2 ml-2 btn btn-danger" v-if="!isNew" v-on:click="onDelete()">Delete</button>
        </div>
    </div>
</template>

<script>
    import VueTagsInput from '@johmun/vue-tags-input';

    import { mapState } from 'vuex'
    import helpers from '../../helpers'

    export default {
        name: 'AgentForm',
        components: {
            editor: require('vue2-ace-editor'),
            VueTagsInput,
        },
        props: {
            isNew: {
                type: Boolean,
                default: false
            }
        },
        data: () => {
            return {
                tag: '',
                tmpTags: [],
                autocompleteItems: []
            }
        },
        computed: {
            filteredItems() {
                return this.autocompleteItems.filter(i => {
                    return i.text.toLowerCase().indexOf(this.tag.toLowerCase()) !== -1;
                })
            },          
            tags: {
                get: function () {
                    if (!this.isNew && this.agent.categories !== undefined) {
                        return this.agent.categories.map(c => {
                            return { text: c };
                        })
                    }

                    return []
                },
                set: function (newValue) {
                    this.tmpTags = newValue
                }
            },
            ...mapState({
                agent: state => state.agents.currentAgent
            })
        },
        async mounted() {
            try {
                const categories = await this.$store.dispatch('agents/categories')
                this.autocompleteItems = categories.map(category => {
                    return { text: category.name };
                })

                this.tmpTags = this.tags
            }
            catch (error) {
                helpers.errorHandle(error)
            }
        },
        methods: {
            onSave() {
                this.agent.categories = this.tmpTags.map(tag => tag.text)

                this.$emit('save', this.agent)
            },
            onUpdate() {
                this.agent.categories = this.tmpTags.map(tag => tag.text)

                this.$emit('update')
            },
            onDelete() {
                this.$emit('delete')
            },
            editorInit: function () {
                require('brace/ext/language_tools')
                require('brace/mode/csharp')
                require('brace/theme/dracula')
                require('brace/snippets/csharp')
            },
            isValid() {
                return this.agent.name && this.agent.command
            }
        }
    }
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
</style>