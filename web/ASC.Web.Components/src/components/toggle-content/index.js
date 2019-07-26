import React from 'react'
import styled from 'styled-components'
import { Icons } from '../icons'
import { Text } from '../text'
import PropTypes from 'prop-types'


const StyledContent = styled.div`
color: #333;
display: ${props => props.isOpen ? 'block' : 'none'};
padding-top: 9px;
`;

const IconArrow= ({ isOpen, ...props }) => <Icons.ArrowContentIcon {...props} />;

const Arrow = styled(IconArrow)`

  margin-right: 9px;
  margin-bottom: 5px;
  transform: ${props => props.isOpen && 'rotate(180deg)'};
`;

const StyledSpan = styled.span`

  cursor: pointer;
  user-select: none;
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
  };

  render() {
    //console.log("ToggleContent render");
    return (
      <div>
        <StyledSpan onClick={() => {
          this.toggleContent(!this.state.isOpen);
          this.props.onChange && this.props.onChange(!this.state.isOpen);
        }}>
          <Arrow color="#333333" isfill={true} size='medium' isOpen={this.state.isOpen} />
          <Text.Headline tag='h2' isInline={true}>{this.props.label}</Text.Headline>
        </StyledSpan>
        <StyledContent isOpen={this.state.isOpen}>{this.props.children}</StyledContent>
      </div>
    )
  }
}

ToggleContent.propTypes = {
  isOpen: PropTypes.bool,
  onChange: PropTypes.func
}

ToggleContent.defaultProps = {
  isOpen: false,
  label: "Some label"
}

export default ToggleContent;
