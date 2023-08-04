import React from "react";
import { RoomsType } from "../../../constants";

import RectangleLoader from "../RectangleLoader";

import { StyledBlock, StyledContainer } from "./StyledFilterBlockLoader";

const FilterBlockLoader = ({
  id,
  className,
  style,
  isRooms,
  isAccounts,

  ...rest
}) => {
  const roomTypeLoader = isRooms ? (
    <>
      {Object.values(RoomsType).map((roomType) => {
        switch (roomType) {
          case RoomsType.FillingFormsRoom:
            return (
              <RectangleLoader
                key={roomType}
                width={"77"}
                height={"28"}
                borderRadius={"16"}
                className={"loader-item tag-item"}
              />
            );
          case RoomsType.EditingRoom:
            return (
              <RectangleLoader
                key={roomType}
                width={"98"}
                height={"28"}
                borderRadius={"16"}
                className={"loader-item tag-item"}
              />
            );
          case RoomsType.ReviewRoom:
            return (
              <RectangleLoader
                key={roomType}
                width={"112"}
                height={"28"}
                borderRadius={"16"}
                className={"loader-item tag-item"}
              />
            );
          case RoomsType.ReadOnlyRoom:
            return (
              <RectangleLoader
                key={roomType}
                width={"73"}
                height={"28"}
                borderRadius={"16"}
                className={"loader-item tag-item"}
              />
            );
          case RoomsType.CustomRoom:
          default:
            return (
              <RectangleLoader
                key={roomType}
                width={"89"}
                height={"28"}
                borderRadius={"16"}
                className={"loader-item tag-item"}
              />
            );
        }
      })}
    </>
  ) : null;

  return (
    <StyledContainer id={id} className={className} style={style} {...rest}>
      {!isRooms && !isAccounts && (
        <StyledBlock>
          <RectangleLoader
            width={"50"}
            height={"16"}
            borderRadius={"3"}
            className={"loader-item"}
          />
          <RectangleLoader
            width={"100%"}
            height={"32"}
            borderRadius={"3"}
            className={"loader-item"}
          />
          <div className="row-loader">
            <RectangleLoader
              width={"16"}
              height={"16"}
              borderRadius={"3"}
              className={"loader-item"}
            />
            <RectangleLoader
              width={"137"}
              height={"20"}
              borderRadius={"3"}
              className={"loader-item"}
            />
          </div>
        </StyledBlock>
      )}

      {!isAccounts && (
        <StyledBlock>
          <RectangleLoader
            width={"51"}
            height={"16"}
            borderRadius={"3"}
            className={"loader-item"}
          />
          <div className="row-loader">
            <RectangleLoader
              width={"51"}
              height={"28"}
              borderRadius={"16"}
              className={"loader-item"}
            />
            <RectangleLoader
              width={"68"}
              height={"28"}
              borderRadius={"16"}
              className={"loader-item"}
            />
          </div>
          {isRooms && (
            <div className="row-loader">
              <RectangleLoader
                width={"16"}
                height={"16"}
                borderRadius={"3"}
                className={"loader-item"}
              />
              <RectangleLoader
                width={"137"}
                height={"20"}
                borderRadius={"3"}
                className={"loader-item"}
              />
            </div>
          )}
        </StyledBlock>
      )}

      {(isRooms || isAccounts) && (
        <StyledBlock>
          <RectangleLoader
            width={"50"}
            height={"16"}
            borderRadius={"3"}
            className={"loader-item"}
          />
          <div className="row-loader">
            {isAccounts ? (
              <>
                <RectangleLoader
                  width={"67"}
                  height={"28"}
                  borderRadius={"16"}
                  className={"loader-item tag-item"}
                />
                <RectangleLoader
                  width={"80"}
                  height={"28"}
                  borderRadius={"16"}
                  className={"loader-item tag-item"}
                />
                <RectangleLoader
                  width={"83"}
                  height={"28"}
                  borderRadius={"16"}
                  className={"loader-item tag-item"}
                />
              </>
            ) : isRooms ? (
              <>{roomTypeLoader}</>
            ) : (
              <></>
            )}
          </div>
        </StyledBlock>
      )}

      {isAccounts && (
        <StyledBlock>
          <RectangleLoader
            width={"50"}
            height={"16"}
            borderRadius={"3"}
            className={"loader-item"}
          />
          <div className="row-loader">
            <RectangleLoader
              width={"114"}
              height={"28"}
              borderRadius={"16"}
              className={"loader-item tag-item"}
            />
            <RectangleLoader
              width={"110"}
              height={"28"}
              borderRadius={"16"}
              className={"loader-item tag-item"}
            />
            <RectangleLoader
              width={"108"}
              height={"28"}
              borderRadius={"16"}
              className={"loader-item tag-item"}
            />
            <RectangleLoader
              width={"59"}
              height={"28"}
              borderRadius={"16"}
              className={"loader-item tag-item"}
            />
          </div>
        </StyledBlock>
      )}

      <StyledBlock isLast>
        <RectangleLoader
          width={"50"}
          height={"16"}
          borderRadius={"3"}
          className={"loader-item"}
        />
        <div className="row-loader">
          {isAccounts ? (
            <>
              <RectangleLoader
                width={"57"}
                height={"28"}
                borderRadius={"16"}
                className={"loader-item tag-item"}
              />
              <RectangleLoader
                width={"57"}
                height={"28"}
                borderRadius={"16"}
                className={"loader-item tag-item"}
              />
            </>
          ) : isRooms ? (
            <>
              <RectangleLoader
                width={"67"}
                height={"28"}
                borderRadius={"16"}
                className={"loader-item tag-item"}
              />
              <RectangleLoader
                width={"73"}
                height={"28"}
                borderRadius={"16"}
                className={"loader-item tag-item"}
              />
              <RectangleLoader
                width={"67"}
                height={"28"}
                borderRadius={"16"}
                className={"loader-item tag-item"}
              />
              <RectangleLoader
                width={"74"}
                height={"28"}
                borderRadius={"16"}
                className={"loader-item tag-item"}
              />
              <RectangleLoader
                width={"65"}
                height={"28"}
                borderRadius={"16"}
                className={"loader-item tag-item"}
              />
              <RectangleLoader
                width={"72"}
                height={"28"}
                borderRadius={"16"}
                className={"loader-item tag-item"}
              />
            </>
          ) : (
            <>
              <RectangleLoader
                width={"73"}
                height={"28"}
                borderRadius={"16"}
                className={"loader-item tag-item"}
              />
              <RectangleLoader
                width={"99"}
                height={"28"}
                borderRadius={"16"}
                className={"loader-item tag-item"}
              />
              <RectangleLoader
                width={"114"}
                height={"28"}
                borderRadius={"16"}
                className={"loader-item tag-item"}
              />
              <RectangleLoader
                width={"112"}
                height={"28"}
                borderRadius={"16"}
                className={"loader-item tag-item"}
              />
              <RectangleLoader
                width={"130"}
                height={"28"}
                borderRadius={"16"}
                className={"loader-item tag-item"}
              />
              <RectangleLoader
                width={"66"}
                height={"28"}
                borderRadius={"16"}
                className={"loader-item tag-item"}
              />
              <RectangleLoader
                width={"81"}
                height={"28"}
                borderRadius={"16"}
                className={"loader-item tag-item"}
              />
              <RectangleLoader
                width={"74"}
                height={"28"}
                borderRadius={"16"}
                className={"loader-item tag-item"}
              />
              <RectangleLoader
                width={"68"}
                height={"28"}
                borderRadius={"16"}
                className={"loader-item tag-item"}
              />
            </>
          )}
        </div>
      </StyledBlock>
    </StyledContainer>
  );
};

export default FilterBlockLoader;
