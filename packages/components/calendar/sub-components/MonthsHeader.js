import React from "react";
import { HeaderActionIcon, HeaderContainer, Title } from "../styled-components";
import { HeaderButtons } from "./HeaderButtons";

export const MonthsHeader = ({
  observedDate,
  setObservedDate,
  setSelectedScene,
  minDate,
  maxDate,
}) => {
  const onTitleClick = () =>
    setSelectedScene((prevSelectedScene) => prevSelectedScene + 1);

  const onLeftClick = () =>
    setObservedDate((prevObservedDate) =>
      prevObservedDate.clone().subtract(1, "year")
    );

  const onRightClick = () =>
    setObservedDate((prevObservedDate) =>
      prevObservedDate.clone().add(1, "year")
    );

  const isLeftDisabled =
    observedDate.clone().subtract(1, "year").endOf("year").endOf("month") <
    minDate;

  const isRightDisabled =
    observedDate.clone().add(1, "year").startOf("year").startOf("month") >
    maxDate;

  return (
    <HeaderContainer>
      <Title className="months-header" onClick={onTitleClick}>
        {observedDate.year()}
        <HeaderActionIcon />
      </Title>
      <HeaderButtons
        onLeftClick={onLeftClick}
        onRightClick={onRightClick}
        isLeftDisabled={isLeftDisabled}
        isRightDisabled={isRightDisabled}
      />
    </HeaderContainer>
  );
};
