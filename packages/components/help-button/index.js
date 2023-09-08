import React from "react";
import PropTypes from "prop-types";
import IconButton from "../icon-button";
import Tooltip from "../tooltip";
import uniqueId from "lodash/uniqueId";
import { classNames } from "../utils/classNames";

import InfoReactSvgUrl from "PUBLIC_DIR/images/info.react.svg?url";

class HelpButton extends React.Component {
  constructor(props) {
    super(props);

    this.id = this.props.id || uniqueId();
  }

  render() {
    const {
      tooltipContent,
      place,
      offset,
      iconName,
      color,
      getContent,
      className,
      dataTip,
      tooltipMaxWidth,
      style,
      size,
      afterShow,
      afterHide,
    } = this.props;

    const anchorSelect = `div[id='${this.id}'] svg`;

    return (
      <div ref={this.ref} style={style}>
        <IconButton
          id={this.id}
          theme={this.props.theme}
          className={classNames(className, "help-icon")}
          isClickable={true}
          iconName={iconName}
          size={size}
          color={color}
          data-for={this.id}
          dataTip={dataTip}
        />

        {getContent ? (
          <Tooltip
            clickable
            openOnClick
            place={place}
            offset={offset}
            afterShow={afterShow}
            afterHide={afterHide}
            maxWidth={tooltipMaxWidth}
            getContent={getContent}
            anchorSelect={anchorSelect}
          />
        ) : (
          <Tooltip
            clickable
            openOnClick
            place={place}
            offset={offset}
            afterShow={afterShow}
            afterHide={afterHide}
            maxWidth={tooltipMaxWidth}
            anchorSelect={anchorSelect}
          >
            {tooltipContent}
          </Tooltip>
        )}
      </div>
    );
  }
}

HelpButton.propTypes = {
  /** Displays the child elements  */
  children: PropTypes.oneOfType([
    PropTypes.arrayOf(PropTypes.node),
    PropTypes.node,
  ]),
  /** Sets the tooltip content  */
  tooltipContent: PropTypes.oneOfType([PropTypes.string, PropTypes.object]),
  /** Required to set additional properties of the tooltip */
  tooltipProps: PropTypes.object,
  /** Sets the maximum width of the tooltip  */
  tooltipMaxWidth: PropTypes.string,
  /** Sets the tooltip id */
  tooltipId: PropTypes.string,
  /** Global tooltip placement */
  place: PropTypes.string,
  /** Specifies the icon name */
  iconName: PropTypes.string,
  /** Icon color */
  color: PropTypes.string,
  /** The data-* attribute is used to store custom data private to the page or application. Required to display a tip over the hovered element */
  dataTip: PropTypes.string,
  /** Sets a callback function that generates the tip content dynamically */
  getContent: PropTypes.func,
  /** Accepts class */
  className: PropTypes.string,
  /** Accepts id */
  id: PropTypes.string,
  /** Accepts css style  */
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
  /** Button height and width value */
  size: PropTypes.number,
};

HelpButton.defaultProps = {
  iconName: InfoReactSvgUrl,
  place: "top",
  className: "icon-button",
  size: 12,
};

export default HelpButton;
