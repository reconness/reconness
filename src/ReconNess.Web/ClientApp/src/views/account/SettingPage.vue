<template>
    <div class="pt-2">
        <h1>Settings</h1>
        <hr />
        <h2>Notification</h2>
        <p class="font-italic"><ins>To know more about settings the notification check the <a href="https://docs.reconness.com/account/settings" target="_blank">documentation</a></ins><p />
        <div class="form-group">
            <label for="inputName">URL</label>
            <input name="url" formControlName="url" class="form-control" id="url" v-model="notification.url">
        </div>
        <div class="form-group">
            <label for="inputName">Method</label>
            <select id="method" class="form-control" v-model="notification.method">
                <option>GET</option>
                <option>POST</option>
            </select>
        </div>
        <div class="form-group">
            <label for="inputName">Payload</label>
            <input name="payload" formControlName="payload" class="form-control" id="payload" v-model="notification.payload">
        </div>
        <div class="form-group">
            <button class="btn btn-primary" v-on:click="onSave" :disabled='!isValid()'>Save</button>
        </div>
    </div>
</template>

<script>
    import helpers from '../../helpers'

    export default {
        name: 'SettingPage',
        data: () => {
            return {
                notification: { url: '', method: '', payload: '' }
            }
        },
        async mounted() {
            const notif = await this.$store.dispatch('accounts/notification')
            if (notif !== '') {
                this.notification = notif
            }
        },
        methods: {
            async onSave() {
                try {
                    await this.$store.dispatch('accounts/saveNotification', this.notification)
                    alert("The settings were saved")
                }
                catch (error) {
                    helpers.errorHandle(error)
                }
            },
            isValid() {
                return this.notification.url && this.notification.method && this.notification.payload
            }
        }
    }
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
</style>