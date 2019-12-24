import React from 'react'
import styled from 'styled-components'
import { Icons } from '../icons'
import Heading from '../heading'
import PropTypes from 'prop-types'


const StyledContent = styled.div`
color: #333;
display: ${props => props.isOpen ? 'block' : 'none'};
padding-top: 9px;
`;

// eslint-disable-next-line react/prop-types, no-unused-vars
const IconArrow = ({ isOpen, ...props }) => <Icons.ArrowContentIcon {...props} />;

const Arrow = styled(IconArrow)`

  margin-right: 9px;
  margin-bottom: 5px;
  transform: ${props => props.isOpen && 'rotate(180deg)'};
`;

const StyledSpan = styled.span`

  cursor: pointer;
  user-select: none;
`;

const StyledText = styled(Heading)`
  height: 26px;
  line-height: 26px;
  box-sizing: border-box;
  font-style: normal;
    &:hover{
    border-bottom: 1px dashed;
  }
`;

class ToggleContent extends React.Component {

  constructor(props) {
    super(props);

    this.state = {
      isOpen: this.props.isOpen
    };
  }

  toggleContent = (isOpen) => this.setState({ isOpen: isOpen });

  componentDidUpdate(prevProps) {
    if (this.props.isOpen !== prevProps.isOpen) {
      this.setState({ isOpen: this.props.isOpen });
    }
  }

  render() {
    //console.log("ToggleContent render");
    return (
      <div
        className={this.props.className}
        id={this.props.id}
        style={this.props.style}
      >
        <StyledSpan onClick={() => {
          this.toggleContent(!this.state.isOpen);
          this.props.onChange && this.props.onChange(!this.state.isOpen);
        }}>
          <Arrow color="#333333" isfill={true} size='medium' isOpen={this.state.isOpen} />
          <StyledText level={2} size='small' isInline={true}>{this.props.label}</StyledText>
        </StyledSpan>
        <StyledContent isOpen={this.state.isOpen}>{this.props.children}</StyledContent>
      </div>
    )
  }
}

ToggleContent.propTypes = {
  label: PropTypes.string.isRequired,
  isOpen: PropTypes.bool,
  onChange: PropTypes.func,
  className: PropTypes.string,
  children: PropTypes.any,
  id: PropTypes.string,
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array])
}

ToggleContent.defaultProps = {
  isOpen: false,
  label: "Some label"
}

export default ToggleContent;
