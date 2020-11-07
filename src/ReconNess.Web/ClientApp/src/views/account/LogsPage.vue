<template>
    <div class="pt-2">
        <loading :active.sync="isLoading"
                 :is-full-page="true"></loading>

        <h1>Logs</h1>
        <hr />
        <div class="form-group">
            <label for="inputName">Log Files</label>
            <select id="method" class="form-control" v-model="logFileSelected" @change="onChange()">
                <option v-for="log in logs" v-bind:value="log" v-bind:key="log">
                    {{ log }}
                </option>
            </select>
        </div>
        <div class="form-group">
            <textarea class="form-control" id="noteFormControl" ref="input" rows="30" v-model="logData"></textarea>
        </div>
        <div class="form-group">
            <button class="btn btn-danger" v-on:click="onClean" :disabled='!isValid()'>Clean</button>
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
        name: 'LogsPage', 
        components: {
            Loading
        },
        data: () => {
            return {
                logs: [],
                logFileSelected: '',
                logData: '',
                isLoading: false
            }
        },        
        async mounted() {
            this.isLoading = true
            try {
                this.logs = await this.$store.dispatch('accounts/logfiles') 
            }
            catch (error) {
                helpers.errorHandle(this.$alert, error)
            }

            this.isLoading = false
        },
        methods: {
            async onChange() {
                this.isLoading = true
                this.logData = await this.$store.dispatch('accounts/readLogfile', this.logFileSelected)
                this.isLoading = false
            },
            async onClean() {
                this.$confirm('Are you sure to clean this log: ' + this.logFileSelected, 'Confirm', 'question').then(async () => {
                    try {
                        this.isLoading = true

                        await this.$store.dispatch('accounts/cleanLogfile', this.logFileSelected)
                        this.logData = ''
                        this.$alert('The log was cleaned', 'Success', 'success')
                    }
                    catch (error) {
                        helpers.errorHandle(this.$alert, error)
                    }

                    this.isLoading = false
                })
            },
            isValid() {
                return this.logFileSelected
            }
        }
    }
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
</style>