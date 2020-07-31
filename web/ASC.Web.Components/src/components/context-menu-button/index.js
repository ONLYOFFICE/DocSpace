import React from 'react'
import styled from 'styled-components'
import PropTypes from 'prop-types'
import DropDownItem from '../drop-down-item'
import DropDown from '../drop-down'
import IconButton from '../icon-button'

const StyledOuter = styled.div`
  display: inline-block;
  position: relative;
  cursor: pointer;
  -webkit-tap-highlight-color: rgba(0, 0, 0, 0);
`;

class ContextMenuButton extends React.Component {
  constructor(props) {
    super(props);

    this.ref = React.createRef();

    this.state = {
      isOpen: props.opened,
      data: props.data
    };
  }

  handleClick = (e) => this.state.isOpen && !this.ref.current.contains(e.target) && this.toggle(false);
  stopAction = (e) => e.preventDefault();
  toggle = (isOpen) => this.setState({ isOpen: isOpen });

  componentDidUpdate(prevProps) {
    if (this.props.opened !== prevProps.opened) {
      this.toggle(this.props.opened);
    }
  }

  onIconButtonClick = () => {
    if (this.props.isDisabled) {
      this.stopAction;
      return;
    }

    this.setState({
      data: this.props.getData(),
      isOpen: !this.state.isOpen
    }, () => !this.props.isDisabled && this.state.isOpen && this.props.onClick && this.props.onClick());
  }

  clickOutsideAction = (e) => {
    if (this.ref.current.contains(e.target)) return;
    
    this.onIconButtonClick();
  }

  onDropDownItemClick = (item, e) => {
    item.onClick && item.onClick(e);
    this.toggle(!this.state.isOpen);
  }

  shouldComponentUpdate(nextProps, nextState) {
    if (this.props.opened === nextProps.opened && this.state.isOpen === nextState.isOpen) {
      return false;
    }
    return true;
  }

  render() {
    //console.log("ContextMenuButton render");
    const {
      className,
      clickColor,
      color,
      columnCount,
      directionX,
      directionY,
      hoverColor,
      iconClickName,
      iconHoverName,
      iconName,
      iconOpenName,
      id,
      isDisabled,
      onMouseEnter,
      onMouseLeave,
      onMouseOut,
      onMouseOver,
      size,
      style
    } = this.props;

    const { isOpen } = this.state;
    const iconButtonName = isOpen && iconOpenName ? iconOpenName : iconName;
    return (
      <StyledOuter ref={this.ref} className={className} id={id} style={style}>
        <IconButton
          color={color}
          hoverColor={hoverColor}
          clickColor={clickColor}
          size={size}
          iconName={iconButtonName}
          iconHoverName={iconHoverName}
          iconClickName={iconClickName}
          isFill={false}
          isDisabled={isDisabled}
          onClick={this.onIconButtonClick}
          onMouseEnter={onMouseEnter}
          onMouseLeave={onMouseLeave}
          onMouseOver={onMouseOver}
          onMouseOut={onMouseOut}
        />
        <DropDown 
          directionX={directionX}
          directionY={directionY}
          open={isOpen}
          clickOutsideAction={this.clickOutsideAction}
          columnCount={columnCount}
        >
          {
            this.state.data.map((item, index) =>
              (item && (item.label || item.icon || item.key)) && <DropDownItem {...item} key={item.key || index} onClick={this.onDropDownItemClick.bind(this, item)}
              />
            )
          }
        </DropDown>
      </StyledOuter>
    );
  }
}

ContextMenuButton.propTypes = {
  opened: PropTypes.bool,
  data: PropTypes.array,
  getData: PropTypes.func.isRequired,
  title: PropTypes.string,
  iconName: PropTypes.string,
  size: PropTypes.number,
  color: PropTypes.string,
  isDisabled: PropTypes.bool,

  hoverColor: PropTypes.string,
  clickColor: PropTypes.string,

  iconHoverName: PropTypes.string,
  iconClickName: PropTypes.string,
  iconOpenName: PropTypes.string,

  onMouseEnter: PropTypes.func,
  onMouseLeave: PropTypes.func,
  onMouseOver: PropTypes.func,
  onMouseOut: PropTypes.func,

  directionX: PropTypes.string,
  directionY: PropTypes.string,

  className: PropTypes.string,
  id: PropTypes.string,
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
  columnCount: PropTypes.number
};

ContextMenuButton.defaultProps = {
  opened: false,
  data: [],
  title: '',
  iconName: 'VerticalDotsIcon',
  size: 16,
  isDisabled: false,
  directionX: 'left'
};

export default ContextMenuButton