import React from 'react'
import PropTypes from 'prop-types'
import DropDownItem from '../drop-down-item'
import DropDown from '../drop-down'

class ContextMenu extends React.PureComponent {
  constructor(props) {
    super(props);

    this.state = {
      visible: false
    };
  }

  componentDidMount() {
    this.container = document.getElementById(this.props.targetAreaId) || document;

    this.container.addEventListener('contextmenu', this.handleContextMenu);
  }

  componentWillUnmount() {
    this.container.removeEventListener('contextmenu', this.handleContextMenu);
  }

  moveMenu = e => {
    const menu = document.getElementById(this.props.id);

    //if (!menu) return;

    const bounds = (this.container !== document) && this.container.getBoundingClientRect();
    const clickX = e.clientX - bounds.left;
    const clickY = e.clientY - bounds.top;
    const containerWidth = this.container.offsetWidth;
    const containerHeight = this.container.offsetHeight;
    const menuWidth = menu && menu.offsetWidth;
    const menuHeight = menu && menu.offsetHeight;
    const right = (containerWidth - clickX) > menuWidth;
    const left = !right;
    const bottom = (containerHeight - clickY) < menuHeight && clickY > menuHeight;

    let newTop = `0px`;
    let newLeft = `0px`;

    newLeft = left ? `${clickX - menuWidth - 8}px` : `${clickX + 8}px`;
    newTop = bottom ? `${clickY - menuHeight}px` : `${clickY}px`;

    if(menu) {
    menu.style.top = newTop;
    menu.style.left = newLeft;
    }
  }

  handleContextMenu = (e) => {
    if (e) {
      e.preventDefault();
      this.handleClick(e);
    }

    this.setState({
      visible: true
    });

    if (e) this.moveMenu(e);
  }

  handleClick = (e) => {
    const { visible } = this.state;
    const menu = document.getElementById(this.props.id);
    const wasOutside = e.target ? !(e.target.contains === menu) : true;

    if (wasOutside && visible)
      this.setState({ visible: false });
  }

  render() {
    const { visible } = this.state;
    const { options, id, className, style } = this.props;

    return (visible && options || null) && (
      <DropDown
        id={id}
        className={className}
        style={style}
        open={true}
        clickOutsideAction={this.handleClick}
      >
        {options.map((item) => {
          if (item && item.key !== undefined) {
            return <DropDownItem key={item.key} {...item} />
          }
        })}
      </DropDown>
    )
  }
}

ContextMenu.propTypes = {
  options: PropTypes.array,
  targetAreaId: PropTypes.string,
  className: PropTypes.string,
  id: PropTypes.string,
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array])
};

ContextMenu.defaultProps = {
  options: [],
  id: "contextMenu"
};

export default ContextMenu;