import type { ChangeEvent } from 'react';

type Option = {
  value: string | number;
  label: string;
};

type FormFieldProps = {
  label: string;
  name: string;
  value: any;
  onChange: (value: any) => void;
  type?: 'text' | 'textarea' | 'select' | 'checkbox' | 'number' | 'email';
  options?: Option[];
  placeholder?: string;
  required?: boolean;
  disabled?: boolean;
};

export const FormField = ({
  label,
  name,
  value,
  onChange,
  type = 'text',
  options = [],
  placeholder,
  required,
  disabled
}: FormFieldProps) => {
  const handleChange = (event: ChangeEvent<HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement>) => {
    if (type === 'checkbox') {
      onChange(event.currentTarget.checked);
    } else if (type === 'number') {
      const numericValue = event.currentTarget.value;
      onChange(numericValue === '' ? '' : Number(numericValue));
    } else {
      onChange(event.currentTarget.value);
    }
  };

  const baseClasses = 'mt-1 block w-full rounded-md border border-slate-200 bg-white px-3 py-2 text-sm shadow-sm focus:border-brand focus:outline-none focus:ring-1 focus:ring-brand disabled:bg-slate-100';

  return (
    <label className="flex flex-col gap-1 text-sm font-medium text-slate-700">
      <span>{label}{required ? ' *' : ''}</span>
      {type === 'textarea' && (
        <textarea
          name={name}
          value={value ?? ''}
          onChange={handleChange}
          placeholder={placeholder}
          disabled={disabled}
          className={`${baseClasses} min-h-[120px]`}
        />
      )}
      {type === 'select' && (
        <select
          name={name}
          value={value ?? ''}
          onChange={handleChange}
          disabled={disabled}
          className={baseClasses}
        >
          <option value="">Seleccione una opción</option>
          {options.map((option) => (
            <option key={option.value} value={option.value}>
              {option.label}
            </option>
          ))}
        </select>
      )}
      {type === 'checkbox' && (
        <input
          type="checkbox"
          name={name}
          checked={Boolean(value)}
          onChange={handleChange}
          disabled={disabled}
          className="h-4 w-4 rounded border-slate-300 text-brand focus:ring-brand"
        />
      )}
      {type !== 'textarea' && type !== 'select' && type !== 'checkbox' && (
        <input
          type={type}
          name={name}
          value={value ?? ''}
          onChange={handleChange}
          placeholder={placeholder}
          disabled={disabled}
          className={baseClasses}
        />
      )}
    </label>
  );
};
