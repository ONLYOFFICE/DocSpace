import React from "react";
import PropTypes from "prop-types";
import equal from "fast-deep-equal/react";

import DropDown from "../drop-down";
import DropDownItem from "../drop-down-item";
import {
  StyledSpan,
  StyledText,
  StyledLinkWithDropdown,
  Caret,
} from "./styled-link-with-dropdown";

class LinkWithDropdown extends React.Component {
  constructor(props) {
    super(props);

    this.state = {
      isOpen: props.isOpen,
    };

    this.ref = React.createRef();
  }

  setIsOpen = (isOpen) => this.setState({ isOpen: isOpen });

  onOpen = () => {
    if (this.props.isDisabled) return;
    this.setIsOpen(!this.state.isOpen);
  };

  onClose = (e) => {
    if (this.ref.current.contains(e.target)) return;

    this.setIsOpen(!this.state.isOpen);
  };

  componentDidUpdate(prevProps) {
    if (this.props.dropdownType !== prevProps.dropdownType) {
      if (this.props.isOpen !== prevProps.isOpen) {
        this.setIsOpen(this.props.isOpen);
      }
    } else if (this.props.isOpen !== prevProps.isOpen) {
      this.setIsOpen(this.props.isOpen);
    }
  }

  onClickDropDownItem = (item) => {
    this.setIsOpen(!this.state.isOpen);
    item.onClick && item.onClick();
  };

  shouldComponentUpdate(nextProps, nextState) {
    return !equal(this.props, nextProps) || !equal(this.state, nextState);
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
      isDisabled,
      ...rest
    } = this.props;

    return (
      <StyledSpan className={className} id={id} style={style} ref={this.ref}>
        <span onClick={this.onOpen}>
          <StyledLinkWithDropdown
            isSemitransparent={isSemitransparent}
            dropdownType={dropdownType}
            color={color}
            isDisabled={isDisabled}
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
              isDisabled={isDisabled}
            >
              {this.props.children}
            </StyledText>
            <Caret
              color={color}
              dropdownType={dropdownType}
              isOpen={this.state.isOpen}
              isDisabled={isDisabled}
            />
          </StyledLinkWithDropdown>
        </span>
        <DropDown
          className="fixed-max-width"
          open={this.state.isOpen}
          withArrow={false}
          clickOutsideAction={this.onClose}
          {...rest}
        >
          {data.map((item) => (
            <DropDownItem
              className="drop-down-item"
              key={item.key}
              {...item}
              onClick={this.onClickDropDownItem.bind(this.props, item)}
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
  dropdownType: PropTypes.oneOf(["alwaysDashed", "appearDashedAfterHover"]),
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
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
  isDisabled: PropTypes.bool,
};

LinkWithDropdown.defaultProps = {
  color: "#333333",
  data: [],
  dropdownType: "alwaysDashed",
  fontSize: "13px",
  isBold: false,
  isSemitransparent: false,
  isTextOverflow: true,
  isOpen: false,
  className: "",
  isDisabled: false,
};

export default LinkWithDropdown;
