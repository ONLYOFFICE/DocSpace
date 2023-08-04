import React from "react";
import { HeaderContainer, Title, HeaderActionIcon } from "../styled-components";
import { HeaderButtons } from "./HeaderButtons";

export const DaysHeader = ({
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
      prevObservedDate.clone().subtract(1, "month")
    );

  const onRightClick = () =>
    setObservedDate((prevObservedDate) =>
      prevObservedDate.clone().add(1, "month")
    );

  const isLeftDisabled =
    observedDate.clone().subtract(1, "month").endOf("month") < minDate;
  const isRightDisabled =
    observedDate.clone().add(1, "month").startOf("month") > maxDate;

  return (
    <HeaderContainer>
      <Title onClick={onTitleClick} className="days-header">
        {observedDate.format("MMMM").charAt(0).toUpperCase() +
          observedDate.format("MMMM").substring(1)}{" "}
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
