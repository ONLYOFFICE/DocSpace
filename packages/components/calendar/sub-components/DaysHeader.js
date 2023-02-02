import { HeaderContainer, Title } from "../styled-components";
import { HeaderButtons } from "./HeaderButtons";

const onLeftClick = (setSelectedDate) =>
  setSelectedDate((prevSelectedDate) =>
    prevSelectedDate.clone().subtract(1, "month")
  );

const onRightClick = (setSelectedDate) =>
  setSelectedDate((prevSelectedDate) =>
    prevSelectedDate.clone().add(1, "month")
  );

export const DaysHeader = ({ selectedDate, setSelectedDate }) => {
  return (
    <HeaderContainer>
      <Title>
        {selectedDate.format("MMMM")} {selectedDate.year()}
      </Title>
      <HeaderButtons
        onLeftClick={() => onLeftClick(setSelectedDate)}
        onRightClick={() => onRightClick(setSelectedDate)}
      />
    </HeaderContainer>
  );
};
