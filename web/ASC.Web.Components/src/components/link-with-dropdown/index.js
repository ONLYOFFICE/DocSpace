import React from "react";
import styled, { css } from "styled-components";
import PropTypes from "prop-types";
import { Icons } from "../icons";
import DropDown from "../drop-down";
import DropDownItem from "../drop-down-item";
import { Text } from "../text";
import { handleAnyClick } from "../../utils/event";

// eslint-disable-next-line no-unused-vars
const SimpleLinkWithDropdown = ({ isBold, fontSize, isTextOverflow, isHovered, isSemitransparent, color, title, dropdownType, data,
  ...props
}) => <a {...props}></a>;

SimpleLinkWithDropdown.propTypes = {
  isBold: PropTypes.bool,
  fontSize: PropTypes.number,
  isTextOverflow: PropTypes.bool,
  isHovered: PropTypes.bool,
  isSemitransparent: PropTypes.bool,
  color: PropTypes.string,
  title: PropTypes.string,
  dropdownType: PropTypes.oneOf(["alwaysDashed", "appearDashedAfterHover"]).isRequired,
  data: PropTypes.array
};

const color = props => props.color;

// eslint-disable-next-line react/prop-types 
const ExpanderDownIcon = ({ isSemitransparent, dropdownType, ...props }) => (<Icons.ExpanderDownIcon {...props} />);

const Caret = styled(ExpanderDownIcon)`
  width: 10px;
  min-width: 10px;
  height: 10px;
  min-height: 10px;
  margin-left: 5px;
  margin-top: -4px;
  
  position: absolute;
  right: 6px;
  top: 0;
  bottom: 0;
  margin: auto;

  path {
    fill: ${color};
  }

  ${props => props.dropdownType === "appearDashedAfterHover" && `opacity: 0`};
`;

const StyledLinkWithDropdown = styled(SimpleLinkWithDropdown)`

  cursor: pointer;
  text-decoration: none;
  user-select: none;
  padding-right: 20px;
  position: relative;
  display: inline-grid;

  color: ${color};

  ${props => props.isSemitransparent && `opacity: 0.5`};
  ${props => props.dropdownType === "alwaysDashed" && `text-decoration:  underline dashed`};

  &:not([href]):not([tabindex]) {
    ${props => props.dropdownType === "alwaysDashed" && `text-decoration:  underline dashed`};
    color: ${color};

    &:hover {
      text-decoration: underline dashed;
      color: ${color};
    }
  }

  :hover {
    color: ${color};

    svg {
      ${props => props.dropdownType === "appearDashedAfterHover" && `position: absolute; opacity: 1`};
      ${props => props.isSemitransparent && `opacity: 0.5`};
    }
  }

`;

const SimpleText = ({ isTextOverflow, fontSize, color, ...props }) => (<Text.Body as="span" {...props} />);
const StyledText = styled(SimpleText)`

  color: ${color};

  ${props => props.isTextOverflow && css`
      display: inline-block;
      max-width: 100%;
    `}
`;

const DataDropDown = ({ data, color, fontSize, title, ...props }) => (
  <DropDown {...props}></DropDown>
);

class LinkWithDropdown extends React.PureComponent {

  constructor(props) {
    super(props);

    this.state = {
      isOpen: false
    };

    this.ref = React.createRef();

    this.handleClick = this.handleClick.bind(this);
    this.toggleDropdown = this.toggleDropdown.bind(this);
    this.onDropDownItemClick = this.onDropDownItemClick.bind(this);

    if (props.isOpen) handleAnyClick(true, this.handleClick);
  }

  handleClick = e =>
    this.state.isOpen &&
    !this.ref.current.contains(e.target) &&
    this.toggleDropdown(false);

  toggleDropdown = isOpen => this.setState({ isOpen });

  clickToDropdown = () => this.setState({ isOpen: !this.state.isOpen });


  componentWillUnmount() {
    handleAnyClick(false, this.handleClick);
  }

  componentDidUpdate(prevProps, prevState) {
    if (this.props.dropdownType !== prevProps.dropdownType) {
      if (this.props.isOpen !== prevProps.isOpen) {
        this.toggleDropdown(this.props.isOpen);
      }
    } else if (this.props.isOpen !== prevProps.isOpen) {
      this.toggleDropdown(this.props.isOpen);
    }

    if (this.state.isOpen !== prevState.isOpen) {
      handleAnyClick(this.state.isOpen, this.handleClick);
    }
  }

  onDropDownItemClick = item => {
    item.onClick && item.onClick();
    this.toggleDropdown(!this.state.isOpen);
  };

  render() {
    console.log("LinkWithDropdown render");

    const {
      isSemitransparent,
      dropdownType,
      isTextOverflow,
      fontSize,
      color,
      isBold,
      title
    } = this.props;
    return (
      <>
        <StyledLinkWithDropdown
          ref={this.ref}
          onClick={this.clickToDropdown}
          isSemitransparent={isSemitransparent}
          dropdownType={dropdownType}
          color={color}
        >
          <StyledText
            isTextOverflow={isTextOverflow}
            truncate={isTextOverflow}
            fontSize={fontSize}
            color={color}
            isBold={isBold}
            title={title}
            dropdownType={dropdownType}
          >
            {this.props.children}
          </StyledText>

          <Caret
            color={color}
            dropdownType={dropdownType}
          />
        </StyledLinkWithDropdown>

        <DataDropDown
          isOpen={this.state.isOpen}
          withArrow={false}
          {...this.props}
        >
          {this.props.data.map(item => (
            <DropDownItem
              key={item.key}
              onClick={this.onDropDownItemClick.bind(this.props, item)}
              {...item}
            />
          ))}
        </DataDropDown>
      </>
    );
  }
}

LinkWithDropdown.propTypes = {
  color: PropTypes.string,
  data: PropTypes.array,
  dropdownType: PropTypes.oneOf(["alwaysDashed", "appearDashedAfterHover"]).isRequired,
  fontSize: PropTypes.number,
  isBold: PropTypes.bool,
  isSemitransparent: PropTypes.bool,
  isTextOverflow: PropTypes.bool,
  title: PropTypes.string,
  isOpen: PropTypes.bool,
  children: PropTypes.any
};

LinkWithDropdown.defaultProps = {
  color: "#333333",
  data: [],
  dropdownType: "alwaysDashed",
  fontSize: 13,
  isBold: false,
  isSemitransparent: false,
  isTextOverflow: true
};

export default LinkWithDropdown;
