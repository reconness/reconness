<template>
    <div class="pt-2">
        <h1>Settings</h1>
        <hr />
        <h3>Notification Settings</h3>
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
        <hr />
        <h3>Notification Payloads</h3>
        <div class="form-group">
            <label for="inputName">New root domain. Use <code v-html="`{{rootDomain}}`"></code> to obtain <code>scriptOutput.rootDomain</code> value</label>
            <input name="rootDomainPayload" formControlName="rootDomainPayload" class="form-control" id="rootDomainPayload" v-model="notification.rootDomainPayload">
        </div>
        <div class="form-group">
            <label for="inputName">New Subdomain. Use <code v-html="`{{domain}}`"></code> to obtain <code>scriptOutput.Subdomain</code> value</label>
            <input name="subdomainPayload" formControlName="subdomainPayload" class="form-control" id="subdomainPayload" v-model="notification.subdomainPayload">
        </div>
        <div class="form-group">
            <label for="inputName">New IpAddress. Use <code v-html="`{{ip}}`"></code> to obtain <code>scriptOutput.Ip</code> value</label>
            <input name="ipAddressPayload" formControlName="ipAddressPayload" class="form-control" id="ipAddressPayload" v-model="notification.ipAddressPayload">
        </div>
        <div class="form-group">
            <label for="inputName">New IsAlive. Use <code v-html="`{{isAlive}}`"></code> to obtain <code>scriptOutput.IsAlive</code> value</label>
            <input name="isAlivePayload" formControlName="isAlivePayload" class="form-control" id="isAlivePayload" v-model="notification.isAlivePayload">
        </div>
        <div class="form-group">
            <label for="inputName">New Has Http Open. Use <code v-html="`{{httpOpen}}`"></code> to obtain <code>scriptOutput.HasHttpOpen</code> value</label>
            <input name="hasHttpOpenPayload" formControlName="hasHttpOpenPayload" class="form-control" id="hasHttpOpenPayload" v-model="notification.hasHttpOpenPayload">
        </div>
        <div class="form-group">
            <label for="inputName">New Takeover. Use <code v-html="`{{takeover}}`"></code> to obtain <code>scriptOutput.Takeover</code> value</label>
            <input name="takeoverPayload" formControlName="takeoverPayload" class="form-control" id="takeoverPayload" v-model="notification.takeoverPayload">
        </div>
        <div class="form-group">
            <label for="inputName">New Directory. Use <code v-html="`{{directory}}`"></code> to obtain <code>scriptOutput.Directory</code> value</label>
            <input name="directoryPayload" formControlName="directoryPayload" class="form-control" id="directoryPayload" v-model="notification.directoryPayload">
        </div>
        <div class="form-group">
            <label for="inputName">New Service and Port. Use <code v-html="`{{service}}`"></code> and <code v-html="`{{port}}`"></code> to obtain <code>scriptOutput.Service</code> and <code>scriptOutput.Port</code> values</label>
            <input name="servicePayload" formControlName="servicePayload" class="form-control" id="servicePayload" v-model="notification.servicePayload">
        </div>
        <div class="form-group">
            <label for="inputName">New Note. Use <code v-html="`{{httpsScreenshot}}`"></code> and <code v-html="`{{httpScreenshot}}`"></code> to obtain <code>scriptOutput.HttpsScreenshotFilePath</code> and <code>scriptOutput.HttpScreenshotFilePath</code> value</label>
            <input name="screenshotPayload" formControlName="screenshotPayload" class="form-control" id="notePayload" v-model="notification.screenshotPayload">
        </div>
        <div class="form-group">
            <label for="inputName">New Note. Use <code v-html="`{{note}}`"></code> to obtain <code>scriptOutput.Note</code> value</label>
            <input name="notePayload" formControlName="notePayload" class="form-control" id="notePayload" v-model="notification.notePayload">
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