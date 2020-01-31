<template>
  <div class="pt-2">
    <div class="form-group">
      <label for="noteFormControl">Notes</label>
      <textarea class="form-control" id="noteFormControl" rows="23" v-model="notes.notes"></textarea>
    </div>
    <div class="form-group">
      <button class="btn btn-primary" v-on:click="onSave()">Save</button>
    </div>
  </div>
</template>

<script>  

  export default {
    name: 'NotesTag',
    props: {
      notes: {
        type: Object,
        required: true
      }
    },
    methods: {
      async onSave() {
        if (this.$route.params.subdomain) {
          await this.$api.create('notes/subdomain/' + this.$route.params.targetName + '/' + this.$route.params.subdomain, this.notes)
        }
        else {
          await this.$api.create('notes/target/' + this.$route.params.targetName, this.notes)
        }
        alert("The notes was saved")
      }
    }
  }
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
</style>