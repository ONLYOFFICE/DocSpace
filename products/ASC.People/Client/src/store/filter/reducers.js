import { SET_FILTER } from "./actions";
import Filter from '../../helpers/filter';

const initialState = {
    filter: Filter.getDefault(),
};

const filterReducer = (state = initialState, action) => {
    switch (action.type) {
        case SET_FILTER:
            return Object.assign({}, state, {
                filter: action.filter
            });
        default:
            return state;
    }
};

export default filterReducer;
