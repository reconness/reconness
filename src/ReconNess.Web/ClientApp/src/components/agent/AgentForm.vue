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
        <h4>Agent Type <span class="text-danger" title="What kind of Agent is this? Run in a Target, RootDomain or Subdomain?">*</span></h4>        
        <div class="form-group form-check">
            <input type="radio" id="isByRootDomain" value="RootDomain" v-model="agent.agentType">
            <label class="form-check-label" for="isByRootDomain">
                RootDomain
            </label>
        </div>
        <div class="form-group form-check">
            <input type="radio" id="isBySubdomain" value="Subdomain" v-model="agent.agentType">
            <label class="form-check-label" for="isBySubdomain">
                Subdomain
            </label>
        </div>
        <hr />
        <h4>Triggers </h4>
        <nav>
            <div class="nav nav-tabs" id="nav-tab" role="tablist">
                <a class="nav-item nav-link active" id="nav-general-tab" data-toggle="tab" href="#nav-general" role="tab" aria-controls="nav-subdomains" aria-selected="true">General</a>
                <a :class="agent.agentType === 'RootDomain' ? 'nav-item nav-link' : 'nav-item nav-link disabled'" id="nav-rootdomain-tab" data-toggle="tab" href="#nav-rootdomain" role="tab" aria-controls="nav-rootdomain" aria-selected="false">RootDomain</a>
                <a :class="agent.agentType === 'Subdomain' ? 'nav-item nav-link' : 'nav-item nav-link disabled'" id="nav-subdomain-tab" data-toggle="tab" href="#nav-subdomain" role="tab" aria-controls="nav-subdomain" aria-selected="false">Subdomain</a>
            </div>
        </nav>
        <div class="tab-content" id="nav-tabContent">
            <div class="tab-pane fade show active" id="nav-general" role="tabpanel" aria-labelledby="nav-general-tab">
                <div class="form-group form-check mt-4">
                    <input class="form-check-input" type="checkbox" id="triggerSkipIfRunBefore" v-model="agent.triggerSkipIfRunBefore">
                    <label class="form-check-label" for="triggerSkipIfRunBefore">
                        Skip if Run Before
                    </label>
                </div>                
            </div>            
            <div class="tab-pane fade" id="nav-rootdomain" role="tabpanel" aria-labelledby="nav-rootdomain-tab">               
                <div class="row mt-4">
                    <div class="form-group col-4">
                        <label for="triggerRootdomainIncExcName">Include/Exclude RegExp RootDomain</label>
                        <select class="form-control" id="triggerRootdomainIncExcName" v-model="agent.triggerRootdomainIncExcName">
                            <option>Include</option>
                            <option>Exclude</option>
                        </select>
                    </div>
                    <div class="form-group col-8">
                        <label for="triggerRootdomainName">RegExp RootDomain</label>
                        <input class="form-control" id="triggerRootdomainName" v-model="agent.triggerRootdomainName">
                    </div>
                </div>
            </div>
            <div class="tab-pane fade" id="nav-subdomain" role="tabpanel" aria-labelledby="nav-subdomain-tab">                
                <div class="form-group form-check mt-4">
                    <input class="form-check-input" type="checkbox" id="triggerSubdomainIsAlive" v-model="agent.triggerSubdomainIsAlive">
                    <label class="form-check-label" for="triggerSubdomainIsAlive">
                        Is Alive
                    </label>
                </div>
                <div class="form-group form-check">
                    <input class="form-check-input" type="checkbox" id="triggerSubdomainIsMainPortal" v-model="agent.triggerSubdomainIsMainPortal">
                    <label class="form-check-label" for="triggerSubdomainIsMainPortal">
                        Is Main Portal
                    </label>
                </div>
                <div class="form-group form-check">
                    <input class="form-check-input" type="checkbox" id="triggerSubdomainHasHttpOrHttpsOpen" v-model="agent.triggerSubdomainHasHttpOrHttpsOpen">
                    <label class="form-check-label" for="triggerSubdomainHasHttpOrHttpsOpen">
                        Has HTTP/HTTPS Open
                    </label>
                </div>
                <div class="row">
                    <div class="form-group col-4">
                        <label for="triggerSubdomainIncExcName">Include/Exclude RegExp Subdomain</label>
                        <select class="form-control" id="triggerSubdomainIncExcName" v-model="agent.triggerSubdomainIncExcName">
                            <option>Include</option>
                            <option>Exclude</option>
                        </select>
                    </div>
                    <div class="form-group col-8">
                        <label for="triggerSubdomainName">RegExp Subdomain</label>
                        <input class="form-control" id="triggerSubdomainName" v-model="agent.triggerSubdomainName">
                    </div>
                </div>
                <div class="row">
                    <div class="form-group col-4">
                        <label for="triggerSubdomainIncExcServicePort">Include/Exclude RegExp Service/Port</label>
                        <select class="form-control" id="triggerSubdomainIncExcServicePort" v-model="agent.triggerSubdomainIncExcServicePort">
                            <option>Include</option>
                            <option>Exclude</option>
                        </select>
                    </div>
                    <div class="form-group col-8">
                        <label for="triggerSubdomainServicePort">RegExp Service/Port</label>
                        <input class="form-control" id="triggerSubdomainServicePort" v-model="agent.triggerSubdomainServicePort">
                    </div>
                </div>
                <div class="row">
                    <div class="form-group col-4">
                        <label for="triggerSubdomainIncExcIP">Include/Exclude RegExp IP</label>
                        <select class="form-control" id="triggerSubdomainIncExcIP" v-model="agent.triggerSubdomainIncExcIP">
                            <option>Include</option>
                            <option>Exclude</option>
                        </select>
                    </div>
                    <div class="form-group col-8">
                        <label for="triggerSubdomainIP">RegExp IP</label>
                        <input class="form-control" id="triggerSubdomainIP" v-model="agent.triggerSubdomainIP">
                    </div>
                </div>
                <div class="row">
                    <div class="form-group col-4">
                        <label for="triggerSubdomainIncExcTechnology">Include/Exclude RegExp Technology</label>
                        <select class="form-control" id="triggerSubdomainIncExcTechnology" v-model="agent.triggerSubdomainIncExcTechnology">
                            <option>Include</option>
                            <option>Exclude</option>
                        </select>
                    </div>
                    <div class="form-group col-8">
                        <label for="triggerSubdomainTechnology">RegExp Technology</label>
                        <input class="form-control" id="triggerSubdomainTechnology" v-model="agent.triggerSubdomainTechnology">
                    </div>
                </div>
                <div class="row">
                    <div class="form-group col-4">
                        <label for="triggerSubdomainIncExcLabel">Include/Exclude RegExp Label</label>
                        <select class="form-control" id="triggerSubdomainIncExcLabel" v-model="agent.triggerSubdomainIncExcLabel">
                            <option>Include</option>
                            <option>Exclude</option>
                        </select>
                    </div>
                    <div class="form-group col-8">
                        <label for="triggerSubdomainLabel">RegExp Label</label>
                        <input class="form-control" id="triggerSubdomainLabel" v-model="agent.triggerSubdomainLabel">
                    </div>
                </div>
            </div>
        </div>   
        <hr />
        <h4>Script</h4>
        <div class="form-group">
            <label for="inputArguments"><a href="https://docs.reconness.com/agents/script-agent">Learn more</a></label>
            <editor v-model="agent.script" @init="editorInit" lang="csharp" theme="dracula" width="800" height="600"></editor>
        </div>
        <div class="form-group">
            <button class="btn btn-primary" v-if="isNew" v-on:click="onSave" :disabled='!isValid()'>Add</button>
            <button class="mt-2 btn btn-primary" v-if="!isNew" v-on:click="onUpdate()" :disabled='!isValid()'>Update</button>
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
                helpers.errorHandle(this.$alert, error)
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
                return this.agent.name && this.agent.command && this.agent.agentType
            }
        }
    }
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
</style>