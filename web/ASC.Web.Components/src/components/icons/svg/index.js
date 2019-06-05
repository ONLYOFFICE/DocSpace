import createStyledIcon from './create-styled-icon';
import OrigPeopleIcon from './people.react.svg';
import OrigCalendarIcon from './calendar.react.svg';
import OrigTreeExpanderDownIcon from './tree-expander-down.react.svg';
import OrigTreeExpanderRightIcon from './tree-expander-right.react.svg';

export const PeopleIcon = createStyledIcon(
  OrigPeopleIcon,
  'PeopleIcon'
);
export const CalendarIcon = createStyledIcon(
  OrigCalendarIcon,
  'CalendarIcon'
);
export const TreeExpanderDownIcon = createStyledIcon(
  OrigTreeExpanderDownIcon,
  'TreeExpanderDownIcon'
);
export const TreeExpanderRightIcon = createStyledIcon(
  OrigTreeExpanderRightIcon,
  'TreeExpanderRight'
);