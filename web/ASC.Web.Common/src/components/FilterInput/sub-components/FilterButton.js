import React from "react";
import { ContextMenuButton } from 'asc-web-components';
import PropTypes from 'prop-types';

class FilterButton extends React.PureComponent {
  render() {
    const { getData, id, isDisabled, iconSize } = this.props;
    //console.log('render FilterButton)
    return (
      <ContextMenuButton
        className='filter-button'
        color='#A3A9AE'
        directionY='bottom'
        getData={getData}
        iconName='RectangleFilterIcon'
        iconOpenName='RectangleFilterClickIcon'
        id={id}
        isDisabled={isDisabled}
        size={iconSize}
        title='Actions'
      ></ContextMenuButton>
    )
  }
}
FilterButton.propTypes = {
  getData: PropTypes.func,
  iconSize: PropTypes.oneOfType([PropTypes.number, PropTypes.string]),
  id: PropTypes.string,
  isDisabled: PropTypes.bool,
}
export default FilterButton