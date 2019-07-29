export const SET_FILTER = 'SET_FILTER';

export function setFilter(filter) {
    return {
      type: SET_FILTER,
      filter
    };
  };