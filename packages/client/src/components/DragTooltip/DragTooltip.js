import React, { useCallback, useEffect, useRef } from "react";
import styled from "styled-components";
import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";
import { Base } from "@docspace/components/themes";

const StyledTooltip = styled.div`
  position: fixed;
  padding: 8px;
  z-index: 250;
  background: ${(props) => props.theme.filesDragTooltip.background};
  border-radius: 6px;
  font-size: 15px;
  font-weight: 600;
  -moz-border-radius: 6px;
  -webkit-border-radius: 6px;
  box-shadow: ${(props) => props.theme.filesDragTooltip.boxShadow};
  -moz-box-shadow: ${(props) => props.theme.filesDragTooltip.boxShadow};
  -webkit-box-shadow: ${(props) => props.theme.filesDragTooltip.boxShadow};

  .tooltip-moved-obj-wrapper {
    display: flex;
    align-items: center;
  }
  .tooltip-moved-obj-icon {
    margin-right: 6px;
  }
  .tooltip-moved-obj-extension {
    color: ${(props) => props.theme.filesDragTooltip.color};
  }
`;

StyledTooltip.defaultProps = { theme: Base };

const DragTooltip = (props) => {
  const tooltipRef = useRef(null);
  const {
    t,
    tooltipOptions,
    iconOfDraggedFile,
    isSingleItem,
    item,
    tooltipPageX,
    tooltipPageY,
  } = props;
  const { filesCount, operationName } = tooltipOptions;
  const { title } = item;

  useEffect(() => {
    setTooltipPosition();
  }, [tooltipPageX, tooltipPageY]);

  const setTooltipPosition = useCallback(() => {
    const tooltip = tooltipRef.current;
    if (tooltip) {
      const margin = 8;
      tooltip.style.left = tooltipPageX + margin + "px";
      tooltip.style.top = tooltipPageY + margin + "px";
    }
  }, [tooltipPageX, tooltipPageY]);

  const renderFileMoveTooltip = useCallback(() => {
    const reg = /^([^\\]*)\.(\w+)/;
    const matches = title.match(reg);

    let nameOfMovedObj, fileExtension;
    if (matches) {
      nameOfMovedObj = matches[1];
      fileExtension = matches.pop();
    } else {
      nameOfMovedObj = title;
    }

    return (
      <div className="tooltip-moved-obj-wrapper">
        {iconOfDraggedFile ? (
          <img
            className="tooltip-moved-obj-icon"
            src={`${iconOfDraggedFile}`}
            alt=""
          />
        ) : null}
        {nameOfMovedObj}
        {fileExtension ? (
          <span className="tooltip-moved-obj-extension">.{fileExtension}</span>
        ) : null}
      </div>
    );
  }, [title, iconOfDraggedFile]);

  const tooltipLabel = tooltipOptions
    ? operationName === "copy"
      ? isSingleItem
        ? t("TooltipElementCopyMessage", { element: filesCount })
        : t("TooltipElementsCopyMessage", { element: filesCount })
      : isSingleItem
      ? renderFileMoveTooltip()
      : t("TooltipElementsMoveMessage", { element: filesCount })
    : t("");

  return <StyledTooltip ref={tooltipRef}>{tooltipLabel}</StyledTooltip>;
};

export default inject(({ filesStore }) => {
  const {
    selection,
    iconOfDraggedFile,
    tooltipOptions,
    tooltipPageX,
    tooltipPageY,
  } = filesStore;

  const isSingleItem = selection.length === 1;

  return {
    item: selection[0],
    isSingleItem,
    tooltipOptions,
    iconOfDraggedFile,

    tooltipPageX,
    tooltipPageY,
  };
})(withTranslation("Files")(observer(DragTooltip)));
