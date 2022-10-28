import React from "react";

import RectangleLoader from "../RectangleLoader";

import { StyledBlock, StyledContainer } from "./StyledFilterBlockLoader";

const FilterBlockLoader = ({
  id,
  className,
  style,
  isPersonalRoom,
  isRooms,
  isAccounts,
  ...rest
}) => {
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

      {!isPersonalRoom && !isAccounts && (
        <StyledBlock>
          <RectangleLoader
            width={"51"}
            height={"16"}
            borderRadius={"3"}
            className={"loader-item"}
          />
          <div className="row-loader">
            <RectangleLoader
              width={"49"}
              height={"28"}
              borderRadius={"16"}
              className={"loader-item"}
            />
            <RectangleLoader
              width={"76"}
              height={"28"}
              borderRadius={"16"}
              className={"loader-item"}
            />
          </div>
          <div className="row-loader">
            <RectangleLoader
              width={"32"}
              height={"32"}
              borderRadius={"6"}
              className={"loader-item"}
            />
            <RectangleLoader
              width={"103"}
              height={"16"}
              borderRadius={"3"}
              className={"loader-item"}
            />
          </div>
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
              <>
                <RectangleLoader
                  width={"77"}
                  height={"28"}
                  borderRadius={"16"}
                  className={"loader-item tag-item"}
                />
                <RectangleLoader
                  width={"98"}
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
                  width={"73"}
                  height={"28"}
                  borderRadius={"16"}
                  className={"loader-item tag-item"}
                />
                <RectangleLoader
                  width={"89"}
                  height={"28"}
                  borderRadius={"16"}
                  className={"loader-item tag-item"}
                />
              </>
            ) : (
              <></>
            )}
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
                width={"114"}
                height={"28"}
                borderRadius={"16"}
                className={"loader-item tag-item"}
              />
              <RectangleLoader
                width={"84"}
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
