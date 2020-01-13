import React from 'react'
import styled from 'styled-components'
import { Icons } from '../icons'
import Heading from '../heading'
import PropTypes from 'prop-types'

const StyledContainer = styled.div`

  .span-toggle-content {
    cursor: pointer;
    user-select: none;
  }

  .heading-toggle-content {
    height: 24px;
    line-height: 26px;
    box-sizing: border-box;
    font-style: normal;

    &:hover {
    border-bottom: 1px dashed;
    }
  }
`;

const StyledContent = styled.div`
color: #333;
display: ${props => props.isOpen ? 'block' : 'none'};
padding-top: 6px;
`;

// eslint-disable-next-line react/prop-types, no-unused-vars
const IconArrow = ({ isOpen, ...props }) => <Icons.ArrowContentIcon {...props} />;

const Arrow = styled(IconArrow)`

  margin-right: 9px;
  margin-bottom: 5px;
  transform: ${props => props.isOpen && 'rotate(180deg)'};
`;

class ToggleContent extends React.Component {

  constructor(props) {
    super(props);

    const { isOpen } = props;

    this.state = {
      isOpen
    };
  }

  toggleContent = () => this.setState({ isOpen: !this.state.isOpen });

  componentDidUpdate(prevProps) {
    const { isOpen } = this.props;
    if (isOpen !== prevProps.isOpen) {
      this.setState({ isOpen });
    }
  }

  render() {
    // console.log("ToggleContent render");

    const {
      children,
      className,
      id,
      label,
      style
    } = this.props;

    const { isOpen } = this.state;

    return (
      <StyledContainer
        className={className}
        id={id}
        style={style}
      >
        <span className='span-toggle-content' onClick={this.toggleContent}>
          <Arrow color="#333" isfill={true} size='medium' isOpen={isOpen} />
          <Heading className='heading-toggle-content' level={2} size='small' isInline={true}>{label}</Heading>
        </span>
        <StyledContent isOpen={isOpen}>{children}</StyledContent>
      </StyledContainer>
    )
  }
}

ToggleContent.propTypes = {
  children: PropTypes.any,
  className: PropTypes.string,
  id: PropTypes.string,
  isOpen: PropTypes.bool,
  label: PropTypes.string.isRequired,
  onChange: PropTypes.func,
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array])
}

ToggleContent.defaultProps = {
  isOpen: false,
  label: ""
}

export default ToggleContent;
