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
`;

class ContextMenuButton extends React.Component {
  constructor(props) {
    super(props);

    this.ref = React.createRef();

    this.state = {
      isOpen: props.opened,
      data: props.data
    };

    this.handleClick = this.handleClick.bind(this);
    this.stopAction = this.stopAction.bind(this);
    this.toggle = this.toggle.bind(this);
    this.onIconButtonClick = this.onIconButtonClick.bind(this);
    this.onDropDownItemClick = this.onDropDownItemClick.bind(this);
  }

  handleClick = (e) => this.state.isOpen && !this.ref.current.contains(e.target) && this.toggle(false);
  stopAction = (e) => e.preventDefault();
  toggle = (isOpen) => this.setState({ isOpen: isOpen });

  componentDidUpdate(prevProps, prevState) {
    if (this.props.opened !== prevProps.opened) {
      this.toggle(this.props.opened);
    }
  }

  onIconButtonClick = () => {
    if (this.props.isDisabled) {
      this.stopAction;
      return;
    }

    console.log("ContextMenuButton onIconButtonClick (isOpen)", this.state.isOpen);

    this.setState({
      data: this.props.getData(),
      isOpen: !this.state.isOpen
    });
  }

  clickOutsideAction = () => {
    console.log("ContextMenuButton clickOutsideAction", );
    this.onIconButtonClick();
  }

  onDropDownItemClick = (item) => {
    item.onClick && item.onClick();
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
      color,
      hoverColor,
      clickColor,
      size,
      iconName,
      iconOpenName,
      iconHoverName,
      iconClickName,
      isDisabled,
      onMouseEnter,
      onMouseLeave,
      onMouseOver,
      onMouseOut,
      directionX,
      className,
      id,
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
          open={isOpen}
          clickOutsideAction={this.clickOutsideAction}
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

  className: PropTypes.string,
  id: PropTypes.string,
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array])
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