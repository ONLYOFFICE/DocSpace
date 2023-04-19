import React from "react";
import PropTypes from "prop-types";
import equal from "fast-deep-equal/react";

import DropDown from "../drop-down";
import DropDownItem from "../drop-down-item";
import {
  StyledSpan,
  StyledText,
  StyledTextWithExpander,
  StyledLinkWithDropdown,
  Caret,
} from "./styled-link-with-dropdown";
import { isMobileOnly } from "react-device-detect";
import Scrollbar from "@docspace/components/scrollbar";
import { ReactSVG } from "react-svg";
import { classNames } from "../utils/classNames";
import ExpanderDownReactSvgUrl from "PUBLIC_DIR/images/expander-down.react.svg?url";

class LinkWithDropdown extends React.Component {
  constructor(props) {
    super(props);

    this.state = {
      isOpen: props.isOpen,
      orientation: window.orientation,
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

  onSetOrientation = () => {
    this.setState({
      orientation: window.orientation,
    });
  };

  componentDidMount() {
    window.addEventListener("orientationchange", this.onSetOrientation);
  }

  componentDidUpdate(prevProps) {
    if (this.props.dropdownType !== prevProps.dropdownType) {
      if (this.props.isOpen !== prevProps.isOpen) {
        this.setIsOpen(this.props.isOpen);
      }
    } else if (this.props.isOpen !== prevProps.isOpen) {
      this.setIsOpen(this.props.isOpen);
    }
  }

  componentWillUnmount() {
    window.removeEventListener("orientationchange", this.onSetOrientation);
  }

  onClickDropDownItem = (e) => {
    const { key } = e.target.dataset;
    const item = this.props.data.find((x) => x.key === key);
    this.setIsOpen(!this.state.isOpen);
    item && item.onClick && item.onClick(e);
  };

  onCheckManualWidth = () => {
    const padding = 32;
    const width = this.ref.current
      ?.querySelector(".text")
      .getBoundingClientRect().width;

    return width + padding + "px";
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
      directionY,
      theme,
      hasScroll,
      withExpander,
      dropDownClassName,
      ...rest
    } = this.props;

    const showScroll =
      hasScroll && isMobileOnly && this.state.orientation === 90;

    const dropDownItem = data.map((item) => (
      <DropDownItem
        className="drop-down-item"
        id={item.key}
        key={item.key}
        {...item}
        onClick={this.onClickDropDownItem}
        data-key={item.key}
        textOverflow={isTextOverflow}
      />
    ));

    const styledText = (
      <StyledText
        className="text"
        isTextOverflow={isTextOverflow}
        truncate={isTextOverflow}
        fontSize={fontSize}
        fontWeight={fontWeight}
        color={color}
        isBold={isBold}
        title={title}
        dropdownType={dropdownType}
        isDisabled={isDisabled}
        withTriangle
      >
        {this.props.children}
      </StyledText>
    );

    return (
      <StyledSpan
        $isOpen={this.state.isOpen}
        className={className}
        id={id}
        style={style}
        ref={this.ref}
      >
        <span onClick={this.onOpen}>
          <StyledLinkWithDropdown
            isSemitransparent={isSemitransparent}
            dropdownType={dropdownType}
            color={color}
            isDisabled={isDisabled}
          >
            {withExpander ? (
              <StyledTextWithExpander isOpen={this.state.isOpen}>
                {styledText}
                <ReactSVG className="expander" src={ExpanderDownReactSvgUrl} />
              </StyledTextWithExpander>
            ) : (
              styledText
            )}
          </StyledLinkWithDropdown>
        </span>
        <DropDown
          className={classNames("fixed-max-width", dropDownClassName)}
          manualWidth={showScroll ? this.onCheckManualWidth() : null}
          open={this.state.isOpen}
          withArrow={false}
          forwardedRef={this.ref}
          directionY={directionY}
          isDropdown={false}
          clickOutsideAction={this.onClose}
          {...rest}
        >
          {showScroll ? (
            <Scrollbar
              className="scroll-drop-down-item"
              style={{
                height: 108,
              }}
            >
              {dropDownItem}
            </Scrollbar>
          ) : (
            dropDownItem
          )}
        </DropDown>
      </StyledSpan>
    );
  }
}

LinkWithDropdown.propTypes = {
  /** Link color in all states - hover, active, visited */
  color: PropTypes.string,
  /** Array of objects, each can contain `<DropDownItem />` props */
  data: PropTypes.array,
  /** Dropdown type 'alwaysDashed' always displays a dotted style and an arrow icon,
   * appearDashedAfterHover displays a dotted style and icon arrow only  on hover */
  dropdownType: PropTypes.oneOf(["alwaysDashed", "appearDashedAfterHover"]),
  /** Displays the expander */
  withExpander: PropTypes.bool,
  /** Link font size */
  fontSize: PropTypes.string,
  /** Link font weight */
  fontWeight: PropTypes.oneOfType([PropTypes.number, PropTypes.string]),
  /** Sets font weight */
  isBold: PropTypes.bool,
  /** Sets css-property 'opacity' to 0.5. Usually applied for the users with "pending" status */
  isSemitransparent: PropTypes.bool,
  /** Activates or deactivates _text-overflow_ CSS property with ellipsis (' … ') value */
  isTextOverflow: PropTypes.bool,
  /** Link title */
  title: PropTypes.string,
  /** Sets open prop */
  isOpen: PropTypes.bool,
  /** Children element */
  children: PropTypes.any,
  /** Accepts css class */
  className: PropTypes.string,
  /** Sets the classNaame of the drop down */
  dropDownClassName: PropTypes.string,
  /** Accepts id */
  id: PropTypes.string,
  /** Accepts css style */
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
  /** Sets disabled view */
  isDisabled: PropTypes.bool,
  /** Sets the opening direction relative to the parent */
  directionY: PropTypes.oneOf(["bottom", "top", "both"]),
  /** Displays the scrollbar */
  hasScroll: PropTypes.bool,
};

LinkWithDropdown.defaultProps = {
  data: [],
  dropdownType: "alwaysDashed",
  fontSize: "13px",
  isBold: false,
  isSemitransparent: false,
  isTextOverflow: true,
  isOpen: false,
  className: "",
  isDisabled: false,
  hasScroll: false,
  withExpander: false,
};

export default LinkWithDropdown;
