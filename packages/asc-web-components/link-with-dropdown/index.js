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

  onClickDropDownItem = (e) => {
    const { key } = e.target.dataset;
    const item = this.props.data.find((x) => x.key === key);
    this.setIsOpen(!this.state.isOpen);
    item && item.onClick && item.onClick(e);
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
              onClick={this.onClickDropDownItem}
              data-key={item.key}
            />
          ))}
        </DropDown>
      </StyledSpan>
    );
  }
}

LinkWithDropdown.propTypes = {
  /** Color of link in all states - hover, active, visited */
  color: PropTypes.string,
  /** Array of objects, each can contain `<DropDownItem />` props */
  data: PropTypes.array,
  /** Type of dropdown: alwaysDashed is always show dotted style and icon of arrow,
   * appearDashedAfterHover is show dotted style and icon arrow only after hover */
  dropdownType: PropTypes.oneOf(["alwaysDashed", "appearDashedAfterHover"]),
  /** Font size of link */
  fontSize: PropTypes.string,
  /** Font weight of link */
  fontWeight: PropTypes.oneOfType([PropTypes.number, PropTypes.string]),
  /** Set font weight */
  isBold: PropTypes.bool,
  /** Set css-property 'opacity' to 0.5. Usually apply for users with "pending" status */
  isSemitransparent: PropTypes.bool,
  /** Activate or deactivate _text-overflow_ CSS property with ellipsis (' … ') value */
  isTextOverflow: PropTypes.bool,
  /** Title of link */
  title: PropTypes.string,
  /** Set open prop */
  isOpen: PropTypes.bool,
  /** Children element */
  children: PropTypes.any,
  /** Accepts css class */
  className: PropTypes.string,
  /** Accepts id */
  id: PropTypes.string,
  /** Accepts css style */
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
  /** Set disabled view */
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
