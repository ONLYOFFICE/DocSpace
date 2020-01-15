import React from "react";
import ContextMenuButton from '../context-menu-button';
import PropTypes from 'prop-types';

class FilterButton extends React.PureComponent {
  render() {
    //console.log('render FilterButton)
    return (
      <ContextMenuButton
        id={this.props.id}
        title='Actions'
        iconName='RectangleFilterIcon'
        iconOpenName='RectangleFilterClickIcon'
        color='#A3A9AE'
        size={this.props.iconSize}
        isDisabled={this.props.isDisabled}
        getData={this.props.getData}

        className='filter-button'
      ></ContextMenuButton>
    )
  }
}
FilterButton.propTypes = {
  id: PropTypes.string,
  iconSize: PropTypes.oneOfType([PropTypes.number, PropTypes.string]),
  isDisabled: PropTypes.bool,
  getData: PropTypes.func
}
export default FilterButton