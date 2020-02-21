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
      <button class="btn btn-primary" v-if="isNew" v-on:click="$emit('save', target)" :disabled='!isValid()'>Add</button>
      <button class="mr-2 mt-2 btn btn-primary" v-if="!isNew" v-on:click="$emit('update')">Update</button>
      <button class="mt-2 btn btn-danger" v-if="!isNew" v-on:click="$emit('delete')">Delete</button>
    </div>    
  </div>
</template>

<script>

  import { mapState } from 'vuex'

  export default {
    name: 'TargetFrom',
    props: {      
      isNew: {
        type: Boolean,
        default: false
      }
    },    
    computed: mapState({
      target: state => state.targets.currentTarget
    }), 
    methods: {
      isValid() {
        return this.target.name && this.target.rootDomain
      }
    }
  }
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>

</style>