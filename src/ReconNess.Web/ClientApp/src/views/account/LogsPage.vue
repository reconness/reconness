<template>
    <div class="pt-2">
        <h1>Logs</h1>
        <hr />
        <div class="form-group">
            <label for="inputName">Log Files</label>
            <select id="method" class="form-control" v-model="logFileSelected" @change="onChange()">
                <option v-for="log in logs" v-bind:value="log">
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
    import helpers from '../../helpers'

    export default {
        name: 'LogsPage', 
        data: () => {
            return {
                logs: [],
                logFileSelected: '',
                logData: ''
            }
        },
        async mounted() {
            this.logs = await this.$store.dispatch('accounts/logfiles')            
        },
        methods: {
            async onChange() {
                this.logData = await this.$store.dispatch('accounts/readLogfile', this.logFileSelected)
            },
            async onClean() {
                if (confirm('Are you sure to clean this Logfile: ' + this.logFileSelected)) {
                    try {
                        await this.$store.dispatch('accounts/cleanLogfile', this.logFileSelected)
                        this.logData = ''
                        alert("The log was cleaned")
                    }
                    catch (error) {
                        helpers.errorHandle(error)
                    }
                }
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