import type { ButtonHTMLAttributes } from 'react';

const baseClasses = 'inline-flex items-center justify-center rounded-md border border-transparent px-4 py-2 text-sm font-semibold transition-colors focus:outline-none focus:ring-2 focus:ring-offset-2 disabled:opacity-60';

const variants: Record<string, string> = {
  primary: 'bg-brand text-white hover:bg-brand-dark focus:ring-brand',
  secondary: 'bg-white text-slate-700 border border-slate-200 hover:bg-slate-100 focus:ring-slate-200',
  ghost: 'bg-transparent text-slate-600 hover:bg-slate-100 focus:ring-slate-200'
};

type ButtonProps = ButtonHTMLAttributes<HTMLButtonElement> & {
  variant?: 'primary' | 'secondary' | 'ghost';
};

export const Button = ({ variant = 'primary', className = '', ...props }: ButtonProps) => {
  const variantClasses = variants[variant] ?? variants.primary;
  return (
    <button
      className={`${baseClasses} ${variantClasses} ${className}`.trim()}
      {...props}
    />
  );
};
