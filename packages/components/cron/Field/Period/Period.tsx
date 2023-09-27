import React, { useMemo, memo } from "react";

import ComboBox from "../../../combobox";
import { getLabel, getOptions } from "./Period.helper";

import PeriodProps, { type PeriodOptionType } from "./Period.props";

function Period({ period = "Hour", setPeriod, t }: PeriodProps) {
  const onSelect = (arg: PeriodOptionType) => {
    setPeriod(arg.key);
  };

  const options = useMemo(() => getOptions(t), [t]);
  const selectedOption = useMemo(
    () => ({ key: period, label: getLabel(period, t) }),
    [period, t]
  );

  return (
    <ComboBox
      scaledOptions
      size="content"
      scaled={false}
      noBorder={false}
      options={options}
      showDisabledItems
      onSelect={onSelect}
      selectedOption={selectedOption}
    />
  );
}

export default memo(Period);
