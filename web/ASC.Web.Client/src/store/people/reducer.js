import { SET_USERS } from "./actions";

const initialState = {
    users: [],
    //groups: [],
    //selection: [],
    //selected: "none",
    //selectedGroup: null,
    //filter: Filter.getDefault(),
    //selector: {
    //  users: []
    //}
  };

const peopleReducer = (state = initialState, action) => {
    switch (action.type) {

      case SET_USERS:
        return Object.assign({}, state, {
          users: action.users
        });
      default:
        return state;
    }
  };

  export default peopleReducer;