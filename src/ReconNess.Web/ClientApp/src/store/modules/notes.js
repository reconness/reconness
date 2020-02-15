import api from '../../api'

const actions = {
    saveTargetNote(context, { targetName, notes}) {
        return new Promise((resolve, reject) => {
            try {                
                console.log(notes)
                api.create('notes/target/' + targetName, { notes: notes })
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
    saveSubdomainNote(context, { targetName, subdomain, notes }) {
        return new Promise((resolve, reject) => {
            try {
                api.create('notes/subdomain/' + targetName + '/' + subdomain, { notes: notes })
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