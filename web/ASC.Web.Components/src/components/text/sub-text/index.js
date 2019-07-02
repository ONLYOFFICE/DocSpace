import createStyledHeadline from './create-styled-headline';
import createStyledBody from './create-styled-body-text';
import createStyledHeader from './create-styled-header';

export const Headline = createStyledHeadline();
export const Body = createStyledBody();
export const MenuHeader = createStyledHeader('MenuHeader');
export const ContentHeader = createStyledHeader('ContentHeader');