import { SubmissionError } from 'redux-form'
import { createUser, updateUser } from '../../../../../../store/services/api'

function submit (values) {
    function successCallback (res) {
      if (res.data && res.data.error) {
        window.alert(res.data.error.message);
      } else {
        console.log(res);
        window.alert('Success');
      }
    }

    function errorCallback (error) {
      throw new SubmissionError({
        _error: error
      })
    }

    if (values.id) {
      updateUser(values).then(successCallback).catch(errorCallback);
    } else {
      createUser(values).then(successCallback).catch(errorCallback);
    }
}

export default submit