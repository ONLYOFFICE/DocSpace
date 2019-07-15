import React from 'react'
import styled from 'styled-components'
import PropTypes from 'prop-types'
import DropDownItem from '../drop-down-item'
import DropDown from '../drop-down'
import IconButton from '../icon-button'

const StyledOuther = styled.div`
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
  }

  handleClick = (e) => !this.ref.current.contains(e.target) && this.toggle(false);
  stopAction = (e) => e.preventDefault();
  toggle = (isOpen) => this.setState({ isOpen: isOpen });

  componentDidMount() {
    if (this.ref.current) {
      document.addEventListener("click", this.handleClick);
    }
  }

  componentWillUnmount() {
    document.removeEventListener("click", this.handleClick)
  }

  componentDidUpdate(prevProps) {
    // Store prevId in state so we can compare when props change.
    // Clear out previously-loaded data (so we don't render stale stuff).
    if (this.props.opened !== prevProps.opened) {
      this.toggle(this.props.opened);
    }
  }

  render() {
    return (
      <StyledOuther ref={this.ref}>
        <IconButton
          color={this.props.color}
          size={this.props.size}
          iconName={this.props.iconName}
          isFill={false}
          isDisabled={this.props.isDisabled}
          onClick={
            !this.props.isDisabled
              ? () => { 
                this.setState({ data: this.props.getData()});
                this.toggle(!this.state.isOpen);
              }
              : this.stopAction
          }
        />
        <DropDown direction={this.props.direction || 'left'} isOpen={this.state.isOpen}>
          {
            this.state.data.map(item => 
              <DropDownItem 
                {...item}
                onClick={() => { 
                  item.onClick && item.onClick();
                  this.toggle(!this.state.isOpen);
                }}
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
  size: PropTypes.oneOf(['small', 'medium', 'big', 'scale']),
  color: PropTypes.string,
  isDisabled: PropTypes.bool
};

ContextMenuButton.defaultProps = {
  opened: false,
  data: [],
  title: '',
  iconName: 'VerticalDotsIcon',
  size: 'medium',
  isDisabled: false
};

export default ContextMenuButton