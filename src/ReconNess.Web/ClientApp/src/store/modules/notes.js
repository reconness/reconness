import api from '../../api'

const actions = {
    saveTargetNote(context, { targetName, rootDomain, notes}) {
        return new Promise((resolve, reject) => {
            try {                
                api.create('notes/target/' + targetName + '/' + rootDomain, { notes: notes })
                    .then(() => {
                        resolve()
                    })
                    .catch(err => reject(err))
            }
            catch (err) {
                reject(err)
            }
        })
    },
    saveSubdomainNote(context, { targetName, rootDomain, subdomain, notes }) {
        return new Promise((resolve, reject) => {
            try {
                api.create('notes/subdomain/' + targetName + '/' + rootDomain + '/' + subdomain, { notes: notes })
                    .then(() => {
                        resolve()
                    })
                    .catch(err => reject(err))
            }
            catch (err) {
                reject(err)
            }
        })
    }
}


export default {
    namespaced: true,
    actions
}