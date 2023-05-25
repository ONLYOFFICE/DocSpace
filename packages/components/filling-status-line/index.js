import React from "react";
import PropTypes from "prop-types";
import { ReactSVG } from "react-svg";
import { mockData } from "./mockData.js";
import { FillingStatusContainer } from "./styled-filling-status-line.js";
import FillingStatusAccordion from "./filling-status-accordion.js";
import StatusDoneReactSvgUrl from "PUBLIC_DIR/images/done.react.svg?url";
import StatusInterruptedSvgUrl from "PUBLIC_DIR/images/interrupted.react.svg?url";

import Text from "../text";
import Box from "../box";

const FillingStatusLine = ({
  statusDoneText,
  statusInterruptedText,
  statusDone,
  statusInterrupted,
}) => {
  return (
    <FillingStatusContainer
      isDone={statusDone}
      isInterrupted={statusInterrupted}
    >
      {mockData.map((data) => {
        return (
          <FillingStatusAccordion
            key={data.id}
            displayName={data.displayName}
            avatar={data.avatar}
            role={data.role}
            startFilling={data.startFillingStatus}
            startFillingDate={data.startFillingDate}
            filledAndSigned={data.filledAndSignedStatus}
            filledAndSignedDate={data.filledAndSignedDate}
            returnedByUser={data.returnedByUser}
            returnedDate={data.returnedByUserDate}
            comment={data.comment}
            isDone={statusDone}
            isInterrupted={statusInterrupted}
          />
        );
      })}
      {statusInterrupted ? (
        <Box displayProp="flex" alignItems="center" marginProp="15px 0 0">
          <ReactSVG
            src={StatusInterruptedSvgUrl}
            className="status-interrupted-icon"
          />
          <Text
            fontSize="14px"
            lineHeight="16px"
            fontWeight="bold"
            className="status-interrupted-text"
          >
            {statusInterruptedText}
          </Text>
        </Box>
      ) : (
        <Box displayProp="flex" alignItems="center" marginProp="15px 0 0">
          <ReactSVG src={StatusDoneReactSvgUrl} className="status-done-icon" />
          <Text
            fontSize="14px"
            lineHeight="16px"
            fontWeight="bold"
            className="status-done-text"
          >
            {statusDoneText}
          </Text>
        </Box>
      )}
    </FillingStatusContainer>
  );
};

FillingStatusLine.propTypes = {
  /** Accepts id */
  id: PropTypes.string,
  /** Accepts class */
  className: PropTypes.string,
  /** Filling status done text*/
  statusDoneText: PropTypes.string,
  /** Filling status interrupted text*/
  statusInterruptedText: PropTypes.string,
  /** Filling status done*/
  statusDone: PropTypes.bool,
  /** Filling status interrupted*/
  statusInterrupted: PropTypes.bool,
};

FillingStatusLine.defaultProps = {
  statusDoneText: "Done",
  statusInterruptedText: "Interrupted",
  statusDone: true,
  statusInterrupted: false,
};

export default FillingStatusLine;
