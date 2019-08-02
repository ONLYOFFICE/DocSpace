import React from 'react'
import styled from 'styled-components'
import PropTypes from 'prop-types'
import DropDownItem from '../drop-down-item'
import DropDown from '../drop-down'
import IconButton from '../icon-button'
import { handleAnyClick } from '../../utils/event';

const StyledOuther = styled.div`
  display: inline-block;
  position: relative;
  cursor: pointer;
`;

class ContextMenuButton extends React.PureComponent {
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

    if(props.opened)
      handleAnyClick(true, this.handleClick);
  }

  handleClick = (e) => this.state.isOpen && !this.ref.current.contains(e.target) && this.toggle(false);
  stopAction = (e) => e.preventDefault();
  toggle = (isOpen) => this.setState({ isOpen: isOpen });

  componentWillUnmount() {
    handleAnyClick(false, this.handleClick);
  }

  componentDidUpdate(prevProps, prevState) {
    // Store prevId in state so we can compare when props change.
    // Clear out previously-loaded data (so we don't render stale stuff).
    if (this.props.opened !== prevProps.opened) {
      this.toggle(this.props.opened);
    }

    if(this.state.isOpen !== prevState.isOpen) {
      handleAnyClick(this.state.isOpen, this.handleClick);
    }
  }

  onIconButtonClick = () => {
    if(!this.props.isDisabled) {
        this.setState({ 
          data: this.props.getData(),
          isOpen: !this.state.isOpen
        });
    }
    else {
      this.stopAction
    }
  }

  onDropDownItemClick = (item) => {
    item.onClick && item.onClick();
    this.toggle(!this.state.isOpen);
  }

  render() {
    //console.log("ContextMenuButton render");
    return (
      <StyledOuther ref={this.ref}>
        <IconButton
          color={this.props.color}
          hoverColor={this.props.hoverColor}
          clickColor={this.props.clickColor}
          size={this.props.size}
          iconName={this.props.iconName}
          iconHoverName={this.props.iconHoverName}
          iconClickName={this.props.iconClickName}
          isFill={false}
          isDisabled={this.props.isDisabled}
          onClick={this.onIconButtonClick}
          onMouseEnter={this.props.onMouseEnter}
          onMouseLeave={this.props.onMouseLeave}
          onMouseOver={this.props.onMouseOver}
          onMouseOut={this.props.onMouseOut}
        />
        <DropDown directionX={this.props.directionX || 'left'} isOpen={this.state.isOpen}>
          {
            this.state.data.map(item =>
              <DropDownItem
                {...item}
                onClick={this.onDropDownItemClick.bind(this, item)}
              />
            )
          }
        </DropDown>
      </StyledOuther>
    );
  };
}

ContextMenuButton.propTypes = {
  opened: PropTypes.bool,
  data: PropTypes.array,
  getData: PropTypes.func.isRequired,
  title: PropTypes.string,
  iconName: PropTypes.string,
  size: PropTypes.number,
  color: PropTypes.string,
  isDisabled: PropTypes.bool
};

ContextMenuButton.defaultProps = {
  opened: false,
  data: [],
  title: '',
  iconName: 'VerticalDotsIcon',
  size: 16,
  isDisabled: false
};

export default ContextMenuButton