<template>
    <div class="pt-2">
        <div class="form-group">
            <label for="noteFormControl">Notes</label>
            <textarea class="form-control" id="noteFormControl" ref="input" rows="20" v-model="notes"></textarea>
        </div>
        <div class="form-group">
            <button class="btn btn-primary" v-on:click="onSave()">Save</button>
        </div>
    </div>
</template>

<script>

    import helpers from '../helpers'

    export default {
        name: 'NotesTag',
        props: {
            isRootDomain: {
                type: Boolean,
                default: false
            }
        },
        data: () => {
            return {
                targetName: '',
                subdomain: ''
            }
        },
        computed: {
            notes: {
                get: function () {
                    if (this.isRootDomain) {
                        return this.$store.state.rootdomains.currentRootDomain.notes
                    }

                    return this.$store.state.subdomains.currentSubdomain.notes
                },
                set: function () {
                }
            }
        },
        mounted() {
            this.targetName = this.$route.params.targetName
            this.rootDomain = this.$route.params.rootDomain
            this.subdomain = this.$route.params.subdomain
        },
        methods: {
            async onSave() {
                try {
                    if (this.isRootDomain) {
                        await this.$store.dispatch('notes/saveTargetNote', {
                            targetName: this.targetName,
                            rootDomain: this.rootDomain,
                            notes: this.$refs.input.value
                        })
                    }
                    else {
                        await this.$store.dispatch('notes/saveSubdomainNote', {
                            targetName: this.targetName,
                            rootDomain: this.rootDomain,
                            subdomain: this.subdomain,
                            notes: this.$refs.input.value
                        })
                    }

                    this.$alert('The notes were saved', 'Success', 'success')
                }
                catch (error) {
                    helpers.errorHandle(this.$alert, error)
                }
            }
        }
    }
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
</style>