<template>
    <div>
        <loading :active.sync="isLoading"
                 :is-full-page="true"></loading>

        <h3>Debug Agent</h3>
        <div class="pt-2">
            <div class="form-group">
                <div class="form-group">
                    <label for="output">Terminal one line output</label>
                    <input name="output" formControlName="output" class="form-control" id="output" v-model="output">
                </div>
                <div class="form-group">
                    <label for="script">Script <a href="https://docs.reconness.com/agents/debug-agent">Learn more</a></label>
                    <editor v-model="content" @init="editorInit" lang="csharp" theme="dracula" width="800" height="600"></editor>
                </div>
            </div>
            <div class="form-group">
                <button class="btn btn-primary" v-on:click="onRun()" :disabled='!isValid()'>Run</button>
            </div>
            <div class="form-group">
                {{result}}
            </div>
        </div>
    </div>
</template>

<script>

    // Import component
    import Loading from 'vue-loading-overlay';
    // Import stylesheet
    import 'vue-loading-overlay/dist/vue-loading.css';

    import helpers from '../../helpers'

    export default {
        name: 'AgentDebugPage',
        components: {
            editor: require('vue2-ace-editor'),
            Loading
        },
        data: () => {
            return {
                content: null,
                output: null,
                result: null,
                isLoading: false
            }
        },
        methods: {
            editorInit: function () {
                require('brace/ext/language_tools')
                require('brace/mode/csharp')
                require('brace/theme/dracula')
                require('brace/snippets/csharp') //snippet
            },
            async onRun() {
                try {
                    this.isLoading = true
                    this.result = await this.$store.dispatch('agents/debug', { terminalOutput: this.output, script: this.content })
                }
                catch (error) {
                    helpers.errorHandle(this.$alert, error)
                }

                this.isLoading = false
            },
            isValid() {
                return this.output && this.content
            }
        }
    }
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
</style>