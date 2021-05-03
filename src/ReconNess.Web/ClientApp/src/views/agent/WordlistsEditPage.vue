<template>
    <div class="pt-2">
        <loading :active.sync="isLoading"
                 :is-full-page="true"></loading>

        <h1>{{$route.params.type}}/{{$route.params.filename}}</h1>
        <div class="form-group">
            <textarea class="form-control" id="noteFormControl" ref="input" rows="30" v-model="data"></textarea>
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
            Loading
        },
        data: () => {
            return {
                data: '',
                isLoading: false
            }
        },        
        async mounted() {
            this.isLoading = true
            try {
                this.data = await this.$store.dispatch('wordlists/getData', { type: this.$route.params.type, filename: this.$route.params.filename }) 
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
            }
        }
    }
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
</style>