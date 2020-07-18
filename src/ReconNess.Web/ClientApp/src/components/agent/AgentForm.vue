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
        <div class="form-group form-check">
            <input class="form-check-input" type="checkbox" id="isBySubdomain" v-model="agent.isBySubdomain" v-on:click="onBySubdomain()">
            <label class="form-check-label" for="isBySubdomain">
                Run by Subdomains
            </label>
        </div>
        <div class="form-group form-check">
            <input class="form-check-input" type="checkbox" id="onlyIfIsAlive" ref="onlyIfIsAlive" v-model="agent.onlyIfIsAlive" :disabled="disabledIsNotBySubdomain">
            <label class="form-check-label" for="onlyIfIsAlive">
                Run Only if it is Alive
            </label>
        </div>
        <div class="form-group form-check">
            <input class="form-check-input" type="checkbox" id="onlyIfHasHttpOne" ref="onlyIfHasHttpOpen" v-model="agent.onlyIfHasHttpOpen" :disabled="disabledIsNotBySubdomain">
            <label class="form-check-label" for="onlyIfHasHttpOpen">
                Run Only if has Http Open
            </label>
        </div>
        <div class="form-group form-check">
            <input class="form-check-input" type="checkbox" id="skipIfRanBefore" ref="skipIfRanBefore" v-model="agent.skipIfRanBefore" :disabled="disabledIsNotBySubdomain">
            <label class="form-check-label" for="skipIfRanBefore">
                Skip If Ran Before
            </label>
        </div>
        <hr />
        <label>Notification Options</label>
        <div class="form-group form-check">
            <input class="form-check-input" type="checkbox" id="notifyIfAgentDone" ref="notifyIfAgentDone" v-model="agent.notifyIfAgentDone">
            <label class="form-check-label" for="notifyIfAgentDone">
                Notify If Agent Done
            </label>
        </div>
        <div class="form-group form-check">
            <input class="form-check-input" type="checkbox" id="notifyNewFound" ref="notifyNewFound" v-model="agent.notifyNewFound">
            <label class="form-check-label" for="notifyNewFound">
                Notify New Found
            </label>
        </div>
        <div>
            <p class="font-italic"><ins>Remember that we need to have setting up the notifications in the <router-link to="/settings">Settings</router-link> Page.</ins></p>
        </div>
        <div class="form-group">
            <label for="inputName">New Subdomain. Use <code v-html="`{{domain}}`"></code> to obtain <code>scriptOutput.Subdomain</code> value</label>
            <input name="subdomainPayload" formControlName="subdomainPayload" class="form-control" id="subdomainPayload" v-model="agent.subdomainPayload" :disabled="disabledIsNotNotif">
        </div>
        <div class="form-group">
            <label for="inputName">New IpAddress. Use <code v-html="`{{ip}}`"></code> to obtain <code>scriptOutput.Ip</code> value</label>
            <input name="ipAddressPayload" formControlName="ipAddressPayload" class="form-control" id="ipAddressPayload" v-model="agent.ipAddressPayload" :disabled="disabledIsNotNotif">
        </div>
        <div class="form-group">
            <label for="inputName">New IsAlive. Use <code v-html="`{{isAlive}}`"></code> to obtain <code>scriptOutput.IsAlive</code> value</label>
            <input name="isAlivePayload" formControlName="isAlivePayload" class="form-control" id="isAlivePayload" v-model="agent.isAlivePayload" :disabled="disabledIsNotNotif">
        </div>
        <div class="form-group">
            <label for="inputName">New Has Http Open. Use <code v-html="`{{httpOpen}}`"></code> to obtain <code>scriptOutput.HasHttpOpen</code> value</label>
            <input name="hasHttpOpenPayload" formControlName="hasHttpOpenPayload" class="form-control" id="hasHttpOpenPayload" v-model="agent.hasHttpOpenPayload" :disabled="disabledIsNotNotif">
        </div>
        <div class="form-group">
            <label for="inputName">New Takeover. Use <code v-html="`{{takeover}}`"></code> to obtain <code>scriptOutput.Takeover</code> value</label>
            <input name="takeoverPayload" formControlName="takeoverPayload" class="form-control" id="takeoverPayload" v-model="agent.takeoverPayload" :disabled="disabledIsNotNotif">
        </div>
        <div class="form-group">
            <label for="inputName">New Directory. Use <code v-html="`{{directory}}`"></code> to obtain <code>scriptOutput.Directory</code> value</label>
            <input name="directoryPayload" formControlName="directoryPayload" class="form-control" id="directoryPayload" v-model="agent.directoryPayload" :disabled="disabledIsNotNotif">
        </div>
        <div class="form-group">
            <label for="inputName">New Service and Port. Use <code v-html="`{{service}}`"></code> and <code v-html="`{{port}}`"></code> to obtain <code>scriptOutput.Service</code> and <code>scriptOutput.Port</code> values</label>
            <input name="servicePayload" formControlName="servicePayload" class="form-control" id="servicePayload" v-model="agent.servicePayload" :disabled="disabledIsNotNotif">
        </div>
        <div class="form-group">
            <label for="inputName">New Note. Use <code v-html="`{{note}}`"></code> to obtain <code>scriptOutput.Note</code> value</label>
            <input name="notePayload" formControlName="notePayload" class="form-control" id="notePayload" v-model="agent.notePayload" :disabled="disabledIsNotNotif">
        </div>
        <hr />
        <div class="form-group" v-if="!isNew">
            <label for="inputArguments">Script <a href="https://docs.reconness.com/agents/script-agent">Learn more</a></label>
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
            disabledIsNotBySubdomain() {
                return !this.agent.isBySubdomain
            },
            disabledIsNotNotif() {
                return this.agent.notifyNewFound !== true
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
            },
            onBySubdomain() {
                this.agent.onlyIfIsAlive = false
                this.agent.onlyIfHasHttpOpen = false
                this.agent.skipIfRanBefore = false
            }
        }
    }
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
</style>