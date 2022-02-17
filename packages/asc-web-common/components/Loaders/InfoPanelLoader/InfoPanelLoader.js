import PropTypes from "prop-types";
import React from "react";

import RectangleLoader from "../RectangleLoader";
import {
    StyledAccessRow,
    StyledInfoRoomBody,
    StyledPropertiesTable,
    StyledPropertyRow,
    StyledSubtitle,
    StyledThumbnail,
    StyledTitle,
} from "./StyledInfoPanelLoader";

const InfoPanelLoader = ({
    id,
    className,
    style,
    isFolder,
    hasThumbnail,
    ...rest
}) => {
    const {} = rest;

    const randomNumber = (min, max) => Math.random() * (max - min) + min;

    const customRectangleLoader = (width, height, rounded) => (
        <RectangleLoader
            width={"" + width}
            height={"" + height}
            borderRadius={"" + rounded}
        />
    );

    const properties = isFolder
        ? [
              { Owner: 19 },
              { Location: 19 },
              { Type: 17.6 },
              { Size: 17.6 },
              { "Date modified": 17.6 },
              { "Last modified by": 19 },
              { "Date creation": 17.6 },
          ]
        : [
              { Owner: 19 },
              { Location: 19 },
              { Type: 17.6 },
              { "File Extension": 17.6 },
              { Size: 17.6 },
              { "Date modified": 17.6 },
              { "Last modified by": 19 },
              { "Date creation": 17.6 },
              { Versions: 17.6 },
              { Comments: 17.6 },
          ];

    return (
        <StyledInfoRoomBody id={id} className={className} style={style}>
            <StyledTitle>
                {customRectangleLoader(32, 32)}
                {customRectangleLoader(250, 22)}
            </StyledTitle>

            <StyledSubtitle>System Properties</StyledSubtitle>

            {hasThumbnail && (
                <StyledThumbnail>
                    {customRectangleLoader(320.2, 200)}
                </StyledThumbnail>
            )}

            <StyledPropertiesTable>
                {properties.map((property) => (
                    <StyledPropertyRow>
                        <div className="property-title">
                            {Object.keys(property)[0]}
                        </div>
                        {customRectangleLoader(
                            randomNumber(125, 150),
                            Object.values(property)[0]
                        )}
                    </StyledPropertyRow>
                ))}
            </StyledPropertiesTable>

            <StyledSubtitle>Who has access</StyledSubtitle>

            <StyledAccessRow>
                {customRectangleLoader(32, 32, 16)}
                <div className="divider"></div>
                {customRectangleLoader(32, 32, 16)}
                {customRectangleLoader(32, 32, 16)}
            </StyledAccessRow>
        </StyledInfoRoomBody>
    );
};

InfoPanelLoader.propTypes = {
    id: PropTypes.string,
    className: PropTypes.string,
    style: PropTypes.object,
};

InfoPanelLoader.defaultProps = {
    id: undefined,
    className: undefined,
    style: undefined,
};

export default InfoPanelLoader;
