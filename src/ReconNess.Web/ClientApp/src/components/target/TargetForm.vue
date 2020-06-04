<template>
    <div class="pt-2">
        <div class="form-group">
            <label for="targetName">Target Name</label>
            <input name="targetName" formControlName="targetName" class="form-control" id="targetName" v-model="target.name">
        </div>
        <div class="form-group" v-for="(rootDomain, k) in rootDomains" :key="k">
            <label for="rootDomain">Root Domain</label>
            <input type="text" class="form-control" v-model="rootDomain.name">
            <span>
                <font-awesome-icon :icon="['fas', 'minus-circle']" @click="remove(k)" v-show="k || ( !k && rootDomains.length > 1)" title="Remove this Root Domain"/>
                <font-awesome-icon :icon="['fas', 'plus-circle']" @click="add(k)" v-show="k == rootDomains.length-1" title="Add Another Root Domain"/>
            </span>
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
            <button class="btn btn-primary" v-if="isNew" v-on:click="$emit('save', target, rootDomains)" :disabled='!isValid()'>Add</button>
            <button class="mr-2 mt-2 btn btn-primary" v-if="!isNew" v-on:click="$emit('update')" :disabled='!isValid()'>Update</button>
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
      target: state => state.targets.currentTarget,
      rootDomains: state => state.targets.currentTarget.rootDomains || [{ name }]
    }), 
    mounted() {
      if (this.isNew) {
          this.$store.state.targets.currentTarget = { rootDomains: [{ name }] }
      }
    },
    methods: {
        isValid() {
            return this.target.name && this.rootDomains[0].name !== ''
        },
        add(index) {
            this.rootDomains.push({ name: '' });
        },
        remove(index) {
            this.rootDomains.splice(index, 1);
        }
    }
  }
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>

</style>