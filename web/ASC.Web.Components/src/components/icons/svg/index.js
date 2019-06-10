import createStyledIcon from './create-styled-icon';
import OrigPeopleIcon from './people.react.svg';
import OrigCalendarIcon from './calendar.react.svg';
import OrigExpanderDownIcon from './expander-down.react.svg';
import OrigExpanderRightIcon from './expander-right.react.svg';

export const PeopleIcon = createStyledIcon(
  OrigPeopleIcon,
  'PeopleIcon'
);
export const CalendarIcon = createStyledIcon(
  OrigCalendarIcon,
  'CalendarIcon'
);
export const ExpanderDownIcon = createStyledIcon(
  OrigExpanderDownIcon,
  'ExpanderDownIcon'
);
export const ExpanderRightIcon = createStyledIcon(
  OrigExpanderRightIcon,
  'ExpanderRight'
);