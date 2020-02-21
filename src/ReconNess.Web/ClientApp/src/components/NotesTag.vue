<template>
  <div class="pt-2">
    <div class="form-group">
      <label for="noteFormControl">Notes</label>
      <textarea class="form-control" id="noteFormControl" ref="input" rows="23" v-model="notes"></textarea>
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
      isTarget: {
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
          if (this.isTarget) {
            return this.$store.state.targets.currentTarget.notes
          }

          return this.$store.state.subdomains.currentSubdomain.notes
        },
        set: function () {
        }
      }
    },
    mounted() {      
      this.targetName = this.$route.params.targetName
      this.subdomain = this.$route.params.subdomain      
    },
    methods: {
      async onSave() {
        try {          
          if (this.isTarget) {
            await this.$store.dispatch('notes/saveTargetNote', {
              targetName: this.targetName,
              notes: this.$refs.input.value
            })
          }
          else {
            await this.$store.dispatch('notes/saveSubdomainNote', {
              targetName: this.targetName,
              subdomain: this.subdomain,
              notes: this.$refs.input.value
            })
          }

          alert("The notes was saved")
        }
        catch (error) {
          helpers.errorHandle(error)
        }
      }
    }
  }
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
</style>