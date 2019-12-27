import React from "react";
import styled, { css } from "styled-components";
import PropTypes from "prop-types";
import { Icons } from "../icons";
import DropDown from "../drop-down";
import DropDownItem from "../drop-down-item";
import Text from "../text";
import { handleAnyClick } from "../../utils/event";
import isEqual from "lodash/isEqual";

// eslint-disable-next-line no-unused-vars
const SimpleLinkWithDropdown = ({ isBold, fontSize, fontWeight, isTextOverflow, isHovered, isSemitransparent, color, title, dropdownType, data,
  ...props
}) => <a {...props}></a>;

SimpleLinkWithDropdown.propTypes = {
  isBold: PropTypes.bool,
  fontSize: PropTypes.number,
  fontWeight: PropTypes.oneOfType([PropTypes.number, PropTypes.string]),
  isTextOverflow: PropTypes.bool,
  isHovered: PropTypes.bool,
  isSemitransparent: PropTypes.bool,
  color: PropTypes.string,
  title: PropTypes.string,
  dropdownType: PropTypes.oneOf(["alwaysDashed", "appearDashedAfterHover"]).isRequired,
  data: PropTypes.array
};

const color = props => props.color;

// eslint-disable-next-line react/prop-types, no-unused-vars
const ExpanderDownIcon = ({ isSemitransparent, dropdownType, isOpen, ...props }) => (<Icons.ExpanderDownIcon {...props} />);

const Caret = styled(ExpanderDownIcon)`
  width: 8px;
  min-width: 8px;
  height: 8px;
  min-height: 8px;
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

  ${props => props.isOpen && `
    bottom: -1px;
    transform: scale(1, -1);
  `}
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

// eslint-disable-next-line react/prop-types, no-unused-vars
const SimpleText = ({ color, ...props }) => (<Text as="span" {...props} />);
const StyledText = styled(SimpleText)`

  color: ${color};

  ${props => props.isTextOverflow && css`
      display: inline-block;
      max-width: 100%;
    `}
`;

const StyledSpan = styled.span`
position: relative;
`;
class LinkWithDropdown extends React.Component {

  constructor(props) {
    super(props);

    this.state = {
      isOpen: props.isOpen
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
    this.toggleDropdown(!this.state.isOpen);
    item.onClick && item.onClick();
  };

  shouldComponentUpdate(nextProps, nextState) {
    return !isEqual(this.props, nextProps) || !isEqual(this.state, nextState);
  }

  render() {
    // console.log("LinkWithDropdown render");

    const {
      isSemitransparent,
      dropdownType,
      isTextOverflow,
      fontSize,
      fontWeight,
      color,
      isBold,
      title,
      className,
      data,
      id,
      style,
      isOpen,
      ...rest
    } = this.props;
    return (
      <StyledSpan
        className={className}
        id={id}
        style={style}
      >
        <span
          ref={this.ref}
          onClick={this.clickToDropdown}
        >
          <StyledLinkWithDropdown

            isSemitransparent={isSemitransparent}
            dropdownType={dropdownType}
            color={color}
          >
            <StyledText
              isTextOverflow={isTextOverflow}
              truncate={isTextOverflow}
              fontSize={fontSize}
              fontWeight={fontWeight}
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
              isOpen={this.state.isOpen}
            />
          </StyledLinkWithDropdown>
        </span>

        <DropDown
          open={this.state.isOpen}
          withArrow={false}
          {...rest}
        >
          {data.map(item => (
            <DropDownItem
              key={item.key}
              onClick={this.onDropDownItemClick.bind(this.props, item)}
              {...item}
            />
          ))}
        </DropDown>
      </StyledSpan>
    );
  }
}

LinkWithDropdown.propTypes = {
  color: PropTypes.string,
  data: PropTypes.array,
  dropdownType: PropTypes.oneOf(["alwaysDashed", "appearDashedAfterHover"]).isRequired,
  fontSize: PropTypes.string,
  fontWeight: PropTypes.oneOfType([PropTypes.number, PropTypes.string]),
  isBold: PropTypes.bool,
  isSemitransparent: PropTypes.bool,
  isTextOverflow: PropTypes.bool,
  title: PropTypes.string,
  isOpen: PropTypes.bool,
  children: PropTypes.any,
  className: PropTypes.string,
  id: PropTypes.string,
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array])
};

LinkWithDropdown.defaultProps = {
  color: "#333333",
  data: [],
  dropdownType: "alwaysDashed",
  fontSize: '13px',
  isBold: false,
  isSemitransparent: false,
  isTextOverflow: true,
  isOpen: false,
  className: "",
};

export default LinkWithDropdown;
