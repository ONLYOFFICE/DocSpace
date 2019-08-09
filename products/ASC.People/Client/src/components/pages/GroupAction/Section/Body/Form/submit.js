import { SubmissionError } from 'redux-form'

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
}

export default submit