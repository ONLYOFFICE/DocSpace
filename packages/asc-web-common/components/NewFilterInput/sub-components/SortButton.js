import React from "react";

import ComboBox from "@appserver/components/combobox";
import DropDownItem from "@appserver/components/drop-down-item";
import IconButton from "@appserver/components/icon-button";

const SortButton = ({ selectedFilterData, getSortData }) => {
  const [isOpen, setIsOpen] = React.useState(false);

  const toggleCombobox = () => {
    setIsOpen((val) => !val);
  };

  const getAdvancedOptions = () => {
    const data = getSortData();
    console.log(selectedFilterData);
    return (
      <>
        {data.map((item) => (
          <DropDownItem key={item.key}>{item.label}</DropDownItem>
        ))}
      </>
    );
  };

  return (
    <ComboBox
      opened={isOpen}
      toggleAction={toggleCombobox}
      className={"sort-combo-box"}
      options={[]}
      selectedOption={{}}
      directionX={"right"}
      scaled={true}
      size={"content"}
      advancedOptions={getAdvancedOptions()}
    >
      <IconButton
        onClick={toggleCombobox}
        iconName="/static/images/sort.react.svg"
        size={16}
      />
    </ComboBox>
  );
};

export default React.memo(SortButton);
