import { HeaderContainer, Title } from "../styled-components";
import { HeaderButtons } from "./HeaderButtons";

const onLeftClick = (setSelectedDate) => {
  setSelectedDate((prevSelectedDate) =>
    prevSelectedDate.clone().subtract(1, "year")
  );
};
const onRightClick = (setSelectedDate) => {
  setSelectedDate((prevSelectedDate) =>
    prevSelectedDate.clone().add(1, "year")
  );
};

export const MonthsHeader = ({ selectedDate, setSelectedDate }) => {
  return (
    <HeaderContainer>
      <Title>{selectedDate.year()}</Title>
      <HeaderButtons
        onLeftClick={() => onLeftClick(setSelectedDate)}
        onRightClick={() => onRightClick(setSelectedDate)}
      />
    </HeaderContainer>
  );
};
