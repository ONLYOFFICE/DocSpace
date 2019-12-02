import createStyledHeadline from './create-styled-headline';
import createStyledHeader from './create-styled-header';

export const Headline = createStyledHeadline();
export { default as Body } from "./create-styled-body-text";
export const MenuHeader = createStyledHeader('MenuHeader');
export const ContentHeader = createStyledHeader('ContentHeader');