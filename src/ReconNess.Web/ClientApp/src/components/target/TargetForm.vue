<template>
  <div class="pt-2">
    <div class="form-group">
      <label for="targetName">Target Name</label>
      <input name="targetName" formControlName="targetName" class="form-control" id="targetName" v-model="target.name">
    </div>
    <div class="form-group">
      <label for="rootDomain">Root Domain</label>
      <input name="rootDomain" formControlName="rootDomain" class="form-control" id="rootDomain" v-model="target.rootDomain">
    </div>
    <div class="form-group">
      <label for="bugBountyProgramUrl">Bug Bounty Program URL</label>
      <input name="bugBountyProgramUrl" formControlName="bugBountyProgramUrl" class="form-control" id="bugBountyProgramUrl" v-model="target.bugBountyProgramUrl">
    </div>
    <div class="form-group form-check">
      <input class="form-check-input" type="checkbox" id="isPrivate" v-model="target.isPrivate">
      <label class="form-check-label" for="isPrivate">
        Is Private Program?
      </label>
    </div>
    <div class="form-group">
      <label for="inScopeFormControl">In Scope</label>
      <textarea class="form-control" id="inScopeFormControl" rows="10" v-model="target.inScope"></textarea>
    </div>
    <div class="form-group">
      <label for="outOfScopeFormControl">Out Of Scope</label>
      <textarea class="form-control" id="outOfScopeFormControl" rows="10" v-model="target.outOfScope"></textarea>
    </div>
    <div class="form-group">
      <button class="btn btn-primary" v-if="isNew" v-on:click="onSave()" :disabled='!isValid()'>Add</button>
      <button class="mr-2 mt-2 btn btn-primary" v-if="!isNew" v-on:click="onUpdate()">Update</button>
      <button class="mt-2 btn btn-danger" v-if="!isNew" v-on:click="onDelete()">Delete</button>
    </div>    
  </div>
</template>

<script>
  export default {
    name: 'TargetFrom',
    props: {
      parentTarget: {
        type: Object,
        required: true
      }
    },
    data: () => {
      return {
        target: {},
        isNew: true
      }
    },
    async mounted() {
      this.target = this.parentTarget || {}
      this.isNew = this.target.name === undefined
    },
    methods: {
      async onSave() {
        await this.$api.create('targets', this.target)
        this.$router.push({ name: 'target', params: { targetName: this.target.name } })
      },
      async onUpdate() {
        await this.$api.update('targets', this.target.id, this.target)
        if (this.$route.params.targetName !== this.target.name) {
          this.$router.push({ name: 'home' })
        }
        else {
          this.$router.go()
        }
      },
      async onDelete() {
        if (confirm('Are you sure to delete this target with all the subdomains and services: ' + this.target.name)) {          
          await this.$api.delete('targets', this.target.name)
          this.$router.push({ name: 'home' })
          this.$router.go()
        }
      },
      isValid() {
        return this.target.name && this.target.rootDomain
      }
    }
  }
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>

</style>