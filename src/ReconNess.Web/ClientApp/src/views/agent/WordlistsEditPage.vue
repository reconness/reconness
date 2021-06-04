<template>
    <div class="pt-2">
        <loading :active.sync="isLoading"
                 :is-full-page="true"></loading>

        <div class="form-group" >
            <strong>Path: </strong><label>{{path}}</label>
        </div>
        <div class="form-group" >
            <editor v-model="data" @init="editorWordlistInit" lang="yaml" theme="dracula" width="800" height="600"></editor>
        </div>

        <router-link to="/agents/wordlists">Back</router-link>
        <div class="form-group mt-4">
            <button class="btn btn-primary" v-on:click="onSave">Save</button>
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
        name: 'WordlistsEditPage', 
        components: {
            editor: require('vue2-ace-editor'),
            Loading
        },
        data: () => {
            return {
                data: '',
                path: '',
                isLoading: false
            }
        },        
        async mounted() {
            this.isLoading = true
            try {
                var result = await this.$store.dispatch('wordlists/getContent', { type: this.$route.params.type, filename: this.$route.params.filename }) 

                this.data = result.data
                this.path = result.path
            }
            catch (error) {
                helpers.errorHandle(this.$alert, error)
            }

            this.isLoading = false
        },
        methods: {
            async onSave() {
                this.$confirm('Are you sure to save: ' + this.$route.params.wordlist + ' file', 'Confirm', 'question').then(async () => {
                    try {
                        this.isLoading = true

                        await this.$store.dispatch('wordlists/save', { type: this.$route.params.type, filename: this.$route.params.filename, data: this.data})
                        this.$alert('The file was saved', 'Success', 'success')
                    }
                    catch (error) {
                        helpers.errorHandle(this.$alert, error)
                    }

                    this.isLoading = false
                })
            },
            editorWordlistInit: function () {
                require('brace/ext/language_tools')
                require('brace/mode/yaml')
                require('brace/theme/dracula')
                require('brace/snippets/yaml')
            }
        }
    }
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
</style>