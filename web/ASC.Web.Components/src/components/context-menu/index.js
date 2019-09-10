import React from 'react'
import PropTypes from 'prop-types'
import { handleAnyClick } from '../../utils/event';

import DropDownItem from '../drop-down-item'
import DropDown from '../drop-down'

class ContextMenu extends React.PureComponent {
  constructor(props) {
    super(props);

    this.state = {
      visible: false
    };

    if(!this.state.visible)
      handleAnyClick(true, this.handleClick);
  }

  componentDidMount() {
    this.container = document.getElementById(this.props.targetAreaId) || document;

    this.container.addEventListener('contextmenu', this.handleContextMenu);
    this.container.addEventListener('scroll', this.handleScroll);
  }

  componentWillUnmount() {
    this.container.removeEventListener('contextmenu', this.handleContextMenu);
    handleAnyClick(false, this.handleClick);
    this.container.removeEventListener('scroll', this.handleScroll);
  }

  handleContextMenu = (e) => {
    e.preventDefault();
    this.handleClick(e);

    this.setState({
      visible: true
    });

    const menu = document.getElementById('contextMenu');
    const bounds = this.container.getBoundingClientRect();

    const clickX = e.clientX - bounds.left;
    const clickY = e.clientY - bounds.top;

    const screenW = this.container.offsetWidth;
    const screenH = this.container.offsetHeight;
    
    const rootW = menu.offsetWidth;
    const rootH = menu.offsetHeight;

    const right = (screenW - clickX) > rootW;
    const left = !right;
    const top = (screenH - clickY) > rootH;
    const bottom = !top;

    if (right) {
      menu.style.left = `${clickX + 5}px`;
    }

    if (left) {
      menu.style.left = `${clickX - rootW - 5}px`;
    }

    if (top) {
      menu.style.top = `${clickY + 5}px`;
    }

    if (bottom) {
      menu.style.top = `${clickY - rootH - 5}px`;
    }
  }

  handleClick = (e) => {
    const { visible } = this.state;
    const menu = document.getElementById('contextMenu');
    const wasOutside = !(e.target.contains === menu);

    if (wasOutside && visible)
      this.setState({ visible: false });
  }

  handleScroll = () => {
    const { visible } = this.state;

    if (visible)
      this.setState({ visible: false });
  };

  render() {
    const { visible } = this.state;
    const { options } = this.props;

    return (visible && options || null) &&
      <DropDown id="contextMenu" opened={true}>
        {options.map((item) => (
          <DropDownItem key={item.key} {...item} />
        ))}
      </DropDown>
  }
}

ContextMenu.propTypes = {
  options: PropTypes.array,
  targetAreaId: PropTypes.string
};

ContextMenu.defaultProps = {
  options: []
};

export default ContextMenu;