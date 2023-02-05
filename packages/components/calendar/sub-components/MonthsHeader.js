import { HeaderContainer, Title } from "../styled-components";
import { HeaderButtons } from "./HeaderButtons";

const onLeftClick = (setObservedDate) => {
  setObservedDate((prevObservedDate) =>
    prevObservedDate.clone().subtract(1, "year")
  );
};
const onRightClick = (setObservedDate) => {
  setObservedDate((prevObservedDate) =>
    prevObservedDate.clone().add(1, "year")
  );
};

export const MonthsHeader = ({
  observedDate,
  setObservedDate,
  setSelectedScene,
}) => {
  return (
    <HeaderContainer>
      <Title
        onClick={() =>
          setSelectedScene((prevSelectedScene) => prevSelectedScene + 1)
        }
      >
        {observedDate.year()}
      </Title>
      <HeaderButtons
        onLeftClick={() => onLeftClick(setObservedDate)}
        onRightClick={() => onRightClick(setObservedDate)}
      />
    </HeaderContainer>
  );
};
